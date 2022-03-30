using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using System.Data;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class LikeCommand : ILikeCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<LikeCommand> _logger;

        public LikeCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<LikeCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger;
        }

        public async Task CreateLikedEntityAsync(EntityLikeDto entityLike, CancellationToken cancellationToken)
        {
            try
            {
                const string query =

                    @"  
	                INSERT INTO  [dbo].[Entity_Like]
                                 ([Entity_Id]
                                 ,[MembershipUser_Id]
                                 ,[CreatedAtUTC])
                    VALUES
                                 (@Entity_Id
                                 ,@MembershipUserId
                                 ,@CreatedAtUTC)";


                var queryDefinition = new CommandDefinition(query, new
                {
                    Entity_Id = entityLike.EntityId,
                    MembershipUserId = entityLike.MembershipUserId,
                    CreatedAtUTC = entityLike.CreatedAtUTC
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);
                if (result != 1)
                {
                    _logger.LogError("Error: User request to create was not successful.", queryDefinition);
                    throw new DBConcurrencyException("Error: User request was not successful.");
                }

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error: User request to create was not successful.");
                throw new DBConcurrencyException("Error: User request was not successful.");
            }
        }

        public async Task DeleteLikedEntityAsync(EntityLikeDto entityLike, CancellationToken cancellationToken)
        {
            try
            {
                const string query =

                    @"  
	                DELETE FROM   [dbo].[Entity_Like]
                    WHERE         
                                  [Entity_Id] = @Entity_Id
                    AND           [MembershipUser_Id] = @MembershipUser_Id";

                var queryDefinition = new CommandDefinition(query, new
                {
                    Entity_Id = entityLike.EntityId,
                    MembershipUser_Id = entityLike.MembershipUserId,
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);

                if (result != 1)
                {
                    _logger.LogError("Error: User request to delete was not successful.", queryDefinition);
                    throw new DBConcurrencyException("Error: User request to delete was not successful.");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error: User request to create was not successful.");
                throw new DBConcurrencyException("Error: User request was not successful.");
            }
        }
    }
}
