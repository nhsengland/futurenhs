using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class UserCommand : IUserCommand
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<UserCommand> _logger;

        public UserCommand(IAzureSqlDbConnectionFactory connectionFactory,ILogger<UserCommand> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

        }

        public async Task CreateInviteUserAsync(GroupInviteDto entityLike, CancellationToken cancellationToken)
        {
            try
            {
                const string query =

                    @"  
	                INSERT INTO  [dbo].[GroupInvite]
                                 ([EmailAddress]
                                 ,[GroupId]
                                 ,[CreatedAtUTC]
                                 ,[ExpiresAtUTC])
                    VALUES
                                 (@EmailAddress
                                 ,@GroupId
                                 ,@CreatedAtUTC
                                 ,@ExpiresAtUTC)";


                var queryDefinition = new CommandDefinition(query, new
                {
                    EmailAddress = entityLike.EmailAddress,
                    GroupId = entityLike.GroupId,
                    CreatedAtUTC = entityLike.CreatedAtUTC,
                    ExpiresAtUTC = entityLike.ExpiresAtUTC
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);
                if (result != 1)
                {
                    _logger.LogError("Error: CreateInviteUserAsync - User request to create was not successful.", queryDefinition);
                    throw new DBConcurrencyException("Error: User request was not successful.");
                }

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error: CreateInviteUserAsync - User request to create was not successful.");
                throw new DBConcurrencyException("Error: User request was not successful.");
            }
        }

        public async Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchUsers(string term, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT    
                                    [{nameof(MemberSearchDetails.Id)}] = mu.[Id],
                                    [{nameof(MemberSearchDetails.Username)}] = [UserName],
                                    [{nameof(MemberSearchDetails.Email)}] = [Email],
                                    [{nameof(MemberSearchDetails.FirstName)}] = [FirstName],
                                    [{nameof(MemberSearchDetails.LastName)}] = [Surname],
                                    [{nameof(MemberSearchDetails.Initials)}] = [Initials],
	                                [{nameof(MemberSearchDetails.Role)}] = mr.[RoleName]
                    
                    FROM			[dbo].[MembershipUser] mu
                    JOIN			MembershipUsersInRoles mir 
                    ON			    mir.UserIdentifier = mu.Id 	
                    JOIN			MembershipRole mr 
                    ON			    mr.Id = mir.RoleIdentifier	
                    WHERE			Email		                LIKE @Term
                    OR			    UserName	                LIKE @Term
                    OR			    FirstName	                LIKE @Term
                    OR			    Surname		                LIKE @Term
                    OR			    FirstName + ' ' + Surname   LIKE @Term
                    AND			    mu.IsDeleted = 0
                    AND			    mu.IsApproved = 1
                    AND			    mu.IsBanned = 0
                    ORDER BY        FirstName ASC
                    OFFSET          @Offset ROWS 
					FETCH NEXT		@Limit ROWS ONLY;

                    SELECT          COUNT(*)
                    FROM			[dbo].[MembershipUser] mu
                    WHERE			Email		                LIKE @Term
                    OR			    UserName	                LIKE @Term
                    OR			    FirstName	                LIKE @Term
                    OR			    Surname		                LIKE @Term
                    OR			    FirstName + ' ' + Surname   LIKE @Term
                    AND			    mu.IsDeleted = 0
                    AND			    mu.IsApproved = 1
                    AND			    mu.IsBanned = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Term = $"%{term}%"
            });

            var results = reader.Read<MemberSearchDetails>();

            var total = Convert.ToUInt32(await reader.ReadFirstAsync<int>());
   

            return (total, results);
        }
    }
}
