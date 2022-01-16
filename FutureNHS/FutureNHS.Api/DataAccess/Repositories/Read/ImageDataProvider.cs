using Dapper;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.Read
{
    public class ImageDataProvider : IImageDataProvider
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<ImageDataProvider> _logger;

        public ImageDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<ImageDataProvider> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<Image> GetImageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string query =
                @"SELECT Id, MediaType, Data
				FROM Image
                WHERE Id = @ImageId AND Deleted = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var image = await dbConnection.QuerySingleAsync<Image>(query, new
            {
                ImageId = id
            });

            return image;
        }
    }
}
