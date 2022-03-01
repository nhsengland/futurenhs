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


        private readonly Uri _blobConnectionUrl;
        private readonly Uri _blobGeoRedundantConnectionUrl;
        private readonly ILogger<BlobStorageProvider> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ISystemClock _systemClock;

        private CloudBlobContainer? _cloudBlobContainer;
        private CloudBlobClient? _cloudBlobClient;

        public Microsoft.Extensions.Internal.ISystemClock SystemClock { get; }
        public Uri PrimaryServiceUrl { get; }
        public Uri GeoRedundantServiceUrl { get; }
        public ILogger<BlobStorageProvider> Logger { get; }
        public IMemoryCache MemoryCache { get; }

        public BlobStorageProvider(ISystemClock systemClock, Uri blobConnectionUrl, Uri blobGeoRedundantConnectionUrl, ILogger<BlobStorageProvider> logger, IMemoryCache memoryCache)
        {
            _blobConnectionUrl = blobConnectionUrl;
            _blobGeoRedundantConnectionUrl = blobGeoRedundantConnectionUrl;
            _memoryCache = memoryCache;
            _systemClock = systemClock;
            _logger = logger;
        }

        public BlobStorageProvider(Microsoft.Extensions.Internal.ISystemClock systemClock, Uri primaryServiceUrl, Uri geoRedundantServiceUrl, ILogger<BlobStorageProvider> logger, IMemoryCache memoryCache)
        {
            SystemClock = systemClock;
            PrimaryServiceUrl = primaryServiceUrl;
            GeoRedundantServiceUrl = geoRedundantServiceUrl;
            Logger = logger;
            MemoryCache = memoryCache;
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

                //var tokenStartsOn = _systemClock.UtcNow;

                //var tokenExpiresOn = tokenStartsOn.AddMinutes(TOKEN_SAS_TIMEOUT_IN_MINUTES);

                //var userDelegationKey = await _memoryCache.GetOrCreateAsync(
                //    $"{nameof(_blobContainerClient.Name)}:UserDelegationKey",
                //    async cacheEntry =>
                //    {
                //        cacheEntry.Priority = CacheItemPriority.High;
                //        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                //        try
                //        {
                //            var azureResponse = await _blobServiceClient.GetUserDelegationKeyAsync(tokenStartsOn, tokenExpiresOn, cancellationToken);

                //            return azureResponse.Value;
                //        }
                //        catch (RequestFailedException ex)
                //        {
                //            _logger?.LogError(ex, "Unable to access the storage endpoint to generate a user delegation key: '{StatusCode} {StatusCodeName}'", ex.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(ex.Status, CultureInfo.InvariantCulture)));

                //            throw;
                //        }
                //    });

                //var writePermission = BlobSasPermissions.All;

                //var blobSasBuilder = new BlobSasBuilder(writePermission, tokenExpiresOn)
                //{
                //    BlobContainerName = _blobContainerClient.Name,
                //    BlobName = blobClient.Name,
                //    Resource = "b",
                //    StartsOn = tokenStartsOn,
                //    ExpiresOn = tokenExpiresOn,
                //    Protocol = SasProtocol.Https,
                //    //PreauthorizedAgentObjectId = set this if we use AAD to authenticate our users, 
                //};

                //var blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
                //{
                //    Sas = blobSasBuilder.ToSasQueryParameters(userDelegationKey, _blobServiceClient.AccountName)
                //};

                //var uri = blobUriBuilder.ToUri();


                // use a CloudBlockBlob because both BlobBlockClient and BlobClient buffer into memory for uploads
                //var blob = new CloudBlockBlob(blobClient.Uri);

                //await blobClient.UploadAsync(stream, cancellationToken);
                blob.Properties.ContentType = contentType;
                await blob.UploadFromStreamAsync(stream, cancellationToken);
                // set the type after the upload, otherwise will get an error that blob does not exist
                // await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

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
                var blobClientOptions = new BlobClientOptions
                {
                    GeoRedundantSecondaryUri = _blobGeoRedundantConnectionUrl
                };
                var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=timblobtest;AccountKey=MONfskUT2sM0cctUTjnXHWRj83jYqXj+tiMSqu+xGRs3dtl4fEQ+yL3gms3z1iAwTGkVQJXByGhKXTp82AK5Aw==;EndpointSuffix=core.windows.net");

                _cloudBlobClient = storageAccount.CreateCloudBlobClient();
                _cloudBlobContainer = _cloudBlobClient.GetContainerReference("files");
                //var managedIdentityCredential = new StorageSharedKeyCredential("timblobtest", "MONfskUT2sM0cctUTjnXHWRj83jYqXj+tiMSqu+xGRs3dtl4fEQ+yL3gms3z1iAwTGkVQJXByGhKXTp82AK5Aw==");
                //_blobServiceClient = new BlobServiceClient(new Uri("https://timblobtest.blob.core.windows.net/"),managedIdentityCredential, blobClientOptions);
                //_blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=timblobtest;AccountKey=MONfskUT2sM0cctUTjnXHWRj83jYqXj+tiMSqu+xGRs3dtl4fEQ+yL3gms3z1iAwTGkVQJXByGhKXTp82AK5Aw==;EndpointSuffix=core.windows.net");
                //var storageAccount = StorageAccount.Parse(_blobConnectionUrl.ToString());

                //var blobServiceClient = storageAccount.CreateCloudBlobClient();

                //var blobServiceClient = new CloudBlobClient()BlobServiceClient("http://127.0.0.1:10000/devstoreaccount1");
                
                //_blobContainerClient = _blobServiceClient.GetBlobContainerClient("files");

                //_blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true;", "files");
                //_blobContainerClient = new BlobContainerClient(_blobConnectionUrl,new DefaultAzureCredential(), blobClientOptions);
            }
        }

    }
}
