using Dapper;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Application.Application.HardCodedSettings;
using FutureNHS.Application.Interfaces;
using FutureNHS.Infrastructure.Models;
using FutureNHS.Infrastructure.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Infrastructure.Repositories.Read.Interfaces;

namespace FutureNHS.Infrastructure.Repositories.Read
{
    public class ImageDataProvider : IImageDataProvider
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;

        public ImageDataProvider(IAzureSqlDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Image> GetImageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string query =
                @"SELECT Id, MediaType, Data
				FROM Image
                WHERE Id = @ImageId AND Deleted = 0";

            Image image;

            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                image = await dbConnection.QuerySingleAsync<Image>(query, new
                {
                    ImageId = id
                });
            }

            return image;
        }
    }
}
