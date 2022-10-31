using System.Diagnostics;
using Azure;
using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using FileServer.PlatformHelpers.Interfaces;

namespace FileServer.PlatformHelpers
{
    public sealed class AzureTableStoreClient : IAzureTableStoreClient
    {
        private readonly ILogger<AzureTableStoreClient>? _logger;

        private readonly TableClient _tableClient;

        public AzureTableStoreClient(string connectionString, string tableName)
        {
            var cloudTableClient = new TableClient(connectionString, tableName, GetTableClientOptions());
            _tableClient = cloudTableClient;
        }

        public async Task AddEntity (AccessTokenEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using var response = await _tableClient.AddEntityAsync(entity, cancellationToken);
                
                if (response.IsError)
                {
                    Debug.Assert(response is not null);

                    _logger?.LogDebug($"Unable to write a record to table storage '{_tableClient.Name}'.  {response.ClientRequestId} - Reported '{response.ReasonPhrase}' with status code: '{response.Status}'");

                    throw new ApplicationException($"Unable to write the record to table storage.  Please consult log files for more information");
                }
            }
            catch (AuthenticationFailedException ex)
            {
                _logger?.LogError(ex, "Unable to authenticate with the Azure Table Storage service using the default credentials.  Please ensure the user account this application is running under has permissions to access the Blob Storage account we are targeting");

                throw;
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, $"Unable to access the Table storage endpoint as the Add request failed: '{ex.Message}'");

                throw;
            }
        }
        public async Task<AccessTokenEntity> ReadEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentNullException(nameof(partitionKey));
            if (string.IsNullOrWhiteSpace(rowKey)) throw new ArgumentNullException(nameof(rowKey));

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await _tableClient.GetEntityAsync<AccessTokenEntity>(partitionKey, rowKey, cancellationToken: cancellationToken);
               
                return result.Value;
            }
            catch (AuthenticationFailedException ex)
            {
                _logger?.LogError(ex, "Unable to authenticate with the Azure Table Storage service using the default credentials.  Please ensure the user account this application is running under has permissions to access the Blob Storage account we are targeting");

                throw;
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, $"Unable to access the Table Storage endpoint as the Read request failed: '{ex.Message}'");

                throw;
            }
        }

        private static TableClientOptions GetTableClientOptions()
        {
            var tableClientOptions = new TableClientOptions();

            // TODO - Set retry options in line with NFRs once they have been established with the client

            tableClientOptions.Retry.Delay = TimeSpan.FromMilliseconds(800);
            tableClientOptions.Retry.MaxDelay = TimeSpan.FromMinutes(1);
            tableClientOptions.Retry.MaxRetries = 5;
            tableClientOptions.Retry.Mode = RetryMode.Exponential;
            tableClientOptions.Retry.NetworkTimeout = TimeSpan.FromSeconds(5);

            tableClientOptions.Diagnostics.IsDistributedTracingEnabled = true;
            tableClientOptions.Diagnostics.IsLoggingContentEnabled = false;
            tableClientOptions.Diagnostics.IsLoggingEnabled = true;
            tableClientOptions.Diagnostics.IsTelemetryEnabled = true;

            return tableClientOptions;
        }
    }
    
    public class AccessTokenEntity : ITableEntity
    {
        public string UserId { get; set; }
        public string FileId { get; set; }
        public DateTimeOffset ExpiresAtUtc { get; set; }
        public string Token { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
