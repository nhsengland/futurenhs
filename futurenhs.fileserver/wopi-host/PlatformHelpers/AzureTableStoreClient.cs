using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using FutureNHS.WOPIHost.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core = Azure.Core;

namespace FutureNHS.WOPIHost.Azure
{
    public interface IAzureTableStoreClient
    {
        Task AddRecordAsync<T>(string tableName, string partitionKey, string rowKey, T record, CancellationToken cancellationToken) where T : IDictionary<string, object>;

        Task<IDictionary<string, object>> ReadRecordAsync(string tableName, string partitionKey, string rowKey, CancellationToken cancellationToken);
    }

    public sealed class AzureTableStoreClient : IAzureTableStoreClient
    {
        private readonly ILogger<AzureTableStoreClient>? _logger;

        private readonly Uri _primaryServiceUrl;
        private readonly Uri _geoRedundantServiceUrl;

        public AzureTableStoreClient(IOptionsSnapshot<AzurePlatformConfiguration> configuration, ILogger<AzureTableStoreClient>? logger)
        {
            _logger = logger;

            if (configuration?.Value?.AzureTableStorage is null) throw new ArgumentNullException(nameof(configuration));

            var tableStorageConfiguration = configuration.Value.AzureTableStorage;

            var primaryServiceUrl = tableStorageConfiguration.PrimaryServiceUrl;
            var geoRedundantServiceUrl = tableStorageConfiguration.GeoRedundantServiceUrl;

            _primaryServiceUrl = primaryServiceUrl ?? throw new ArgumentNullException(nameof(configuration));
            _geoRedundantServiceUrl = geoRedundantServiceUrl ?? throw new ArgumentNullException(nameof(configuration));
        }

        private static bool IsSuccessStatusCode(int statusCode) => statusCode >= 200 && statusCode <= 299;

        async Task IAzureTableStoreClient.AddRecordAsync<T>(string tableName, string partitionKey, string rowKey, T record, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentNullException(nameof(partitionKey));
            if (string.IsNullOrWhiteSpace(rowKey)) throw new ArgumentNullException(nameof(rowKey));
            if (record is null) throw new ArgumentNullException(nameof(record));
            if (0 >= record.Count) throw new ArgumentOutOfRangeException("The record does not contain any data");
            if (record.ContainsKey("PartitionKey")) throw new ArgumentOutOfRangeException("The record cannot contain a key called PartitionKey");
            if (record.ContainsKey("RowKey")) throw new ArgumentOutOfRangeException("The record cannot contain a key called RowKey");

            cancellationToken.ThrowIfCancellationRequested();
            
            var managedIdentityCredential = new DefaultAzureCredential();

            var tableClientOptions = GetTableClientOptions(_geoRedundantServiceUrl);

            var tableServiceClient = new TableServiceClient(_primaryServiceUrl, managedIdentityCredential, tableClientOptions);

            var tableClient = tableServiceClient.GetTableClient(tableName);

            var entity = new TableEntity(partitionKey, rowKey);

            foreach (var kvp in record) entity.Add(kvp.Key, kvp.Value);

            try
            {
                using var response = await tableClient.AddEntityAsync(entity, cancellationToken);

                if (!IsSuccessStatusCode(response?.Status ?? int.MinValue))
                {
                    Debug.Assert(response is not null);

                    _logger?.LogDebug("Unable to write a record to table storage '{TableName}'.  {ClientRequestId} - Reported '{ReasonPhrase}' with status code: '{StatusCode} {StatusCodeName}'", response.ClientRequestId, tableName, response.ReasonPhrase, response.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(response.Status, CultureInfo.InvariantCulture)));

                    // TODO - Add custom table storage exception

                    throw new ApplicationException($"{response.ClientRequestId}: Unable to write the record to table storage '{tableName}'.  Please consult log files for more information");
                }
            }
            catch (AuthenticationFailedException ex)
            {
                _logger?.LogError(ex, "Unable to authenticate with the Azure Table Storage service using the default credentials.  Please ensure the user account this application is running under has permissions to access the Blob Storage account we are targeting");

                throw;
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, "Unable to access the Table storage endpoint as the Add request failed: '{StatusCode} {StatusCodeName}'", ex.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(ex.Status, CultureInfo.InvariantCulture)));

                throw;
            }
        }
        async Task<IDictionary<string, object>> IAzureTableStoreClient.ReadRecordAsync(string tableName, string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentNullException(nameof(partitionKey));
            if (string.IsNullOrWhiteSpace(rowKey)) throw new ArgumentNullException(nameof(rowKey));

            cancellationToken.ThrowIfCancellationRequested();

            var managedIdentityCredential = new DefaultAzureCredential();

            var tableClientOptions = GetTableClientOptions(_geoRedundantServiceUrl);

            var tableServiceClient = new TableServiceClient(_primaryServiceUrl, managedIdentityCredential, tableClientOptions);

            var tableClient = tableServiceClient.GetTableClient(tableName);

            try
            {
                var result = await tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey, cancellationToken: cancellationToken);
                
                using var response = result.GetRawResponse();

                if (!IsSuccessStatusCode(response?.Status ?? int.MinValue))
                {
                    Debug.Assert(response is not null);

                    _logger?.LogDebug("Unable to read a record from table storage '{TableName}'.  {ClientRequestId} - Reported '{ReasonPhrase}' with status code: '{StatusCode} {StatusCodeName}'", response.ClientRequestId, tableName, response.ReasonPhrase, response.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(response.Status, CultureInfo.InvariantCulture)));

                    // TODO - Add custom table storage exception

                    throw new ApplicationException($"{response.ClientRequestId}: Unable to read a record from table storage '{tableName}'.  Please consult log files for more information");
                }

                return result.Value;
            }
            catch (AuthenticationFailedException ex)
            {
                _logger?.LogError(ex, "Unable to authenticate with the Azure Table Storage service using the default credentials.  Please ensure the user account this application is running under has permissions to access the Blob Storage account we are targeting");

                throw;
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, "Unable to access the Table Storage endpoint as the Read request failed: '{StatusCode} {StatusCodeName}'", ex.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(ex.Status, CultureInfo.InvariantCulture)));

                throw;
            }
        }



        private static TableClientOptions GetTableClientOptions(Uri geoRedundantServiceUrl)
        {
            var tableClientOptions = new TableClientOptions(); // { GeoRedundantSecondaryUri = geoRedundantServiceUrl };

            // TODO - Set retry options in line with NFRs once they have been established with the client

            tableClientOptions.Retry.Delay = TimeSpan.FromMilliseconds(800);
            tableClientOptions.Retry.MaxDelay = TimeSpan.FromMinutes(1);
            tableClientOptions.Retry.MaxRetries = 5;
            tableClientOptions.Retry.Mode = Core.RetryMode.Exponential;
            tableClientOptions.Retry.NetworkTimeout = TimeSpan.FromSeconds(5);

            tableClientOptions.Diagnostics.IsDistributedTracingEnabled = true;
            tableClientOptions.Diagnostics.IsLoggingContentEnabled = false;
            tableClientOptions.Diagnostics.IsLoggingEnabled = true;
            tableClientOptions.Diagnostics.IsTelemetryEnabled = true;

            return tableClientOptions;
        }
    }
}
