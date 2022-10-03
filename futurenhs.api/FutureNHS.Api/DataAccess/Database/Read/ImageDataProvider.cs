using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.Database.Read
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
                @$"SELECT 
                        [{nameof(Image.Id)}]		                = Image.Id,    
                        [{nameof(Image.MediaType)}]		            = Image.MediaType,
                        [{nameof(Image.Data)}]		                = Image.Data
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
