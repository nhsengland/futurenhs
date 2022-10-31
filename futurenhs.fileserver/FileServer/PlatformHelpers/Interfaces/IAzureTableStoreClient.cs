using Azure.Data.Tables;

namespace FileServer.PlatformHelpers.Interfaces
{
    public interface IAzureTableStoreClient
    {
        Task AddEntity(AccessTokenEntity entity, CancellationToken cancellationToken);

        Task<AccessTokenEntity> ReadEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);
    }
}
