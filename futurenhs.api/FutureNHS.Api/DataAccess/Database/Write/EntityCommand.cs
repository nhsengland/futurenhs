using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class EntityCommand : IEntityCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<EntityCommand> _logger;

        public EntityCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<EntityCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger;
        }

        public async Task CreateEntityAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            const string query =
                   @"  
                    BEGIN TRAN
                    BEGIN TRY

	                INSERT INTO  [dbo].[Entity]
                                 ([Id])
                    VALUES
                                 (@EntityId)
	
	                COMMIT TRAN;

                    END TRY
                    BEGIN CATCH
	                    PRINT ERROR_MESSAGE();
	                    ROLLBACK TRAN;
                    END CATCH
                   ";

            var queryDefinition = new CommandDefinition(query, new
            {
                EntityId = entityId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to create was not successful.", queryDefinition);
                throw new DBConcurrencyException("Error: User request to create was not successful.");
            }
        }
    }
}

