using System.Diagnostics;
using FileServer.Models;
using FileServer.PlatformHelpers.Interfaces;
using FileServer.Services.Interfaces;
using FutureNHS.WOPIHost.Configuration;
using Microsoft.Extensions.Options;

namespace FileServer.Services
{
    public sealed class FileContentMetadataService : IFileContentMetadataService
    {
        private readonly IAzureBlobStoreClient _azureBlobStoreClient;
        private readonly ILogger<FileContentMetadataService>? _logger;

        private readonly string _blobContainerName;

        public FileContentMetadataService(IAzureBlobStoreClient azureBlobStoreClient, IOptionsSnapshot<AzurePlatformConfiguration> azurePlatformConfiguration, ILogger<FileContentMetadataService>? logger)
        {
            _logger = logger;

            _azureBlobStoreClient = azureBlobStoreClient ?? throw new ArgumentNullException(nameof(azureBlobStoreClient));

            if (azurePlatformConfiguration?.Value is null) throw new ArgumentNullException(nameof(azurePlatformConfiguration));

            var blobContainerName = azurePlatformConfiguration.Value.AzureBlobStorage?.ContainerName;

            if (string.IsNullOrWhiteSpace(blobContainerName)) throw new ApplicationException("The files blob container name is not set in the configuration");

            _blobContainerName = blobContainerName;
        }

        public async Task<string?> SaveFileAsync(Stream stream,string fileName,string contentType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _azureBlobStoreClient.UploadFileAsync(stream, fileName, contentType, cancellationToken);
        }

        public async Task<FileContentMetadata> GetDetailsAndPutContentIntoStreamAsync(UserFileMetadata fileMetadata, Stream streamToWriteTo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (fileMetadata is null) throw new ArgumentNullException(nameof(fileMetadata));

            Debug.Assert(!string.IsNullOrWhiteSpace(fileMetadata.BlobName));
            Debug.Assert(fileMetadata.ContentHash is not null);

            if (streamToWriteTo is null) throw new ArgumentNullException(nameof(streamToWriteTo));

            Debug.Assert(streamToWriteTo.CanWrite);

            var downloadDetails = await _azureBlobStoreClient.FetchBlobAndWriteToStream(_blobContainerName, fileMetadata.BlobName, fileMetadata.FileVersion, streamToWriteTo, fileMetadata.ContentHash, cancellationToken);

            return new FileContentMetadata(
                contentVersion: downloadDetails.VersionId,
                contentHash: downloadDetails.ContentHash,               
                contentEncoding: downloadDetails.ContentEncoding,
                contentLanguage: downloadDetails.ContentLanguage,
                contentType: downloadDetails.ContentType,
                contentLength: 0 > downloadDetails.ContentLength ? 0 : (ulong)downloadDetails.ContentLength,
                lastAccessed: DateTimeOffset.MinValue == downloadDetails.LastAccessed ? default : downloadDetails.LastAccessed,
                lastModified: downloadDetails.LastModified,
                fileMetadata: fileMetadata
                );
        }
    }
}
