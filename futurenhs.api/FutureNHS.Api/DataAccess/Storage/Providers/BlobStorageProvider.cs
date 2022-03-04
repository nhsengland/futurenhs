using System.Globalization;
using System.Net;
using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Caching.Memory;

namespace FutureNHS.Api.DataAccess.Storage.Providers
{
    public sealed class BlobStorageProvider : IFileBlobStorageProvider, IImageBlobStorageProvider
    {
        const int TOKEN_SAS_TIMEOUT_IN_MINUTES = 40;                 // Aligns with authentication cookie timeout policy for which we have an NFR


        public readonly string _connectionString;
        public readonly string _containerName;
        private readonly ILogger<BlobStorageProvider> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ISystemClock _systemClock;

        private CloudBlobContainer? _cloudBlobContainer;
        private CloudBlobClient? _cloudBlobClient;


        public BlobStorageProvider(ISystemClock systemClock, string connectionString, string containerName, ILogger<BlobStorageProvider> logger, IMemoryCache memoryCache)
        {
            _connectionString = connectionString;
            _containerName = containerName;
            _memoryCache = memoryCache;
            _systemClock = systemClock;
            _logger = logger;
        }

        public async Task UploadFileAsync(Stream stream, string blobName, string contentType, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));

                cancellationToken.ThrowIfCancellationRequested();

                if (_cloudBlobContainer is null)
                {
                    throw new InvalidOperationException("A connection to blob storage was not opened");
                }

                var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);

                blob.Properties.ContentType = contentType;
                await blob.UploadFromStreamAsync(stream, cancellationToken);
            }
            catch (AuthenticationFailedException ex)
            {
                _logger?.LogError(ex, "Unable to authenticate with the Azure Blob Storage service using the default credentials.  Please ensure the user account this application is running under has permissions to access the Blob Storage account we are targeting");

                throw;
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, "Unable to access the storage endpoint as the download request failed: '{StatusCode} {StatusCodeName}'", ex.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(ex.Status, CultureInfo.InvariantCulture)));

                throw;
            }

            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unable to access the storage endpoint as the download request failed:'");

                throw;
            }
        }

        public async Task DeleteFileAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));


            if (_cloudBlobContainer is null)
            {
                throw new InvalidOperationException("A connection to blob storage was not opened");
            }

            // use a CloudBlockBlob because both BlobBlockClient and BlobClient buffer into memory for uploads
            var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);

            await blob.DeleteIfExistsAsync();
        }

        public async Task CreateConnectionAsync()
        {
            if (_cloudBlobClient is null)
            {
                var storageAccount = CloudStorageAccount.Parse(_connectionString);

                _cloudBlobClient = storageAccount.CreateCloudBlobClient();
                _cloudBlobContainer = _cloudBlobClient.GetContainerReference(_containerName);
            }
        }

    }
}
