using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using System.Data;

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
            const string query =

                @"  
                    BEGIN TRAN
                    BEGIN TRY

	                INSERT INTO  [dbo].[Entity_Like]
                                 ([Comment_Id]
                                 ,[Entity_Id]
                                 ,[MembershipUser_Id]
                                 ,[CreatedAtUTC])
                    VALUES
                                 (@Comment_Id
                                 ,@Entity_Id
                                 ,@MembershipUserId
                                 ,@CreatedAtUTC)
	
	                COMMIT TRAN;

                    END TRY
                    BEGIN CATCH
	                    PRINT ERROR_MESSAGE();
	                    ROLLBACK TRAN;
                    END CATCH";

            var queryDefinition = new CommandDefinition(query, new
            {
                Comment_Id = entityLike.CommentId,
                Entity_Id = entityLike.EntityId,
                MembershipUserId = entityLike.MembershipUserId,
                CreatedAtUTC = entityLike.CreatedAtUTC
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to create was not successful.", queryDefinition);
                throw new DBConcurrencyException("Error: User request to create was not successful.");
            }
        }

        public async Task DeleteLikedEntityAsync(EntityLikeDto entityLike, CancellationToken cancellationToken)
        {
            const string query =

                @"  
                    BEGIN TRAN
                    BEGIN TRY

	                DELETE FROM   [dbo].[Entity_Like]
                    WHERE         
                                  [Comment_Id] = @Comment_Id
                    AND           [MembershipUser_Id] = @MembershipUser_Id
					
	                COMMIT TRAN;

                    END TRY
                    BEGIN CATCH
	                    PRINT ERROR_MESSAGE();
	                    ROLLBACK TRAN;
                    END CATCH
                ";

            var queryDefinition = new CommandDefinition(query, new
            {
                Comment_Id = entityLike.CommentId,
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
    }
}
