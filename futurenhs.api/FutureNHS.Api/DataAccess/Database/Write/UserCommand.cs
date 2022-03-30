using System.Data;
using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using Microsoft.Data.SqlClient;

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
    }
}
