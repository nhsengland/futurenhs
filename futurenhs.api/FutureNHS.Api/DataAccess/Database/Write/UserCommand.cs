using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class UserCommand : IUserCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<UserCommand> _logger;

        public UserCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<UserCommand> logger)
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

        public async Task<MemberData?> GetMemberAsync(Guid id, CancellationToken cancellationToken)
        {
            const string query =
                @$" SELECT 
                                [{nameof(MemberData.Id)}]                = u.Id, 
                                [{nameof(MemberData.FirstName)}]         = u.FirstName,
                                [{nameof(MemberData.Surname)}]          = u.Surname,
                                [{nameof(MemberData.ImageId)}]           = u.ImageId, 
                                [{nameof(MemberData.RowVersion)}]        = u.RowVersion
				    
                    FROM        [MembershipUser] u
                    WHERE      
                                 [Id] = @Id";


            var queryDefinition = new CommandDefinition(query, new
            {
                Id = id,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var user = await dbConnection.QueryFirstOrDefaultAsync<MemberData?>(queryDefinition);

            return user;
        }

        public async Task UpdateUserAsync(MemberDto user, byte[] rowVersion, CancellationToken cancellationToken)
        {
            const string query =
                @$" UPDATE        [dbo].[MembershipUser]
                    SET 
                                  [FirstName] = @FirstName
                                 ,[Surname] = @LastName
                                 ,[Pronouns] = @Pronouns
                                 ,[ModifiedAtUTC] = @ModifiedAtUtc
                    
                    WHERE 
                                 [Id] = @Id
                    AND          [RowVersion] = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.Surname,
                Pronouns = user.Pronouns,
                ModifiedAtUTC = user.ModifiedAtUTC,
                ModifiedBy = user.ModifiedBy,
                RowVersion = rowVersion,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to edit user was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to edit user was not successful");
            }
        }
    }
}
