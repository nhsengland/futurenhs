using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class ImageCommand :IImageCommand
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<ImageCommand> _logger;

        public ImageCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<ImageCommand> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<Guid> CreateImageAsync(ImageDto image, CancellationToken cancellationToken = default)
        {
            const string query =
                 @" INSERT INTO     [dbo].[Image]
                                    ([FileName]
                                    ,[FileSizeBytes]
                                    ,[Height]
                                    ,[Width]
                                    ,[MediaType]
                                    ,[CreatedBy]
                                    ,[CreatedAtUtc])
                    OUTPUT          INSERTED.[Id]
                    VALUES
                                    (
                                    @FileName,
                                    @FileSizeBytes,
                                    @Height,
                                    @Width,
                                    @MediaType,
                                    @CreatedBy,
                                    @CreatedAtUtc            
                                    )";

            var queryDefinition = new CommandDefinition(query, new
            {
                Id = image.Id,
                FileSizeBytes = image.FileSizeBytes,
                Height = image.Height,
                Width = image.Width,
                FileName = image.FileName,
                MediaType = image.MediaType,
                CreatedAtUtc = image.CreatedAtUtc,
                CreatedBy = image.CreatedBy,

            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteScalarAsync<Guid?>(queryDefinition);

            if (result.HasValue is false)
            {
                _logger.LogError($"Error: Creation of Image:{0}, by User:{1} failed", image.FileName, image.CreatedBy);
                throw new DBConcurrencyException("Error: Creation of Image failed");
            }

            return result.Value;
        }

        public async Task ForceDeleteImageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string query =
                 @" DELETE FROM     [dbo].[Image]
                    WHERE           Id = @Id";

            var queryDefinition = new CommandDefinition(query, new
            {
                Id = id

            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError($"Error: Force delete of Image:{0} failed", id);
                throw new DBConcurrencyException("Error: Failed to clean up image");
            }
        }
    }
}
