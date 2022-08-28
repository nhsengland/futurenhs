using FutureNHS.WOPIHost.Azure;
using FutureNHS.WOPIHost.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    public interface IFileContentMetadataRepository
    {
        /// <summary>
        /// Tasked with retrieving a file located in storage and writing it into <paramref name="streamToWriteTo"/>
        /// </summary>
        /// <param name="fileMetadata">The metadata pertinent to the file whose contents we are going to try and write to the stream</param>
        /// <param name="streamToWriteTo">The stream to which the content of the file will be written in the success case/></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Information on the content written to the stream</returns>
        Task<FileContentMetadata> GetDetailsAndPutContentIntoStreamAsync(UserFileMetadata fileMetadata, Stream streamToWriteTo, CancellationToken cancellationToken);
    }

    public sealed class FileContentMetadataRepository : IFileContentMetadataRepository
    {
        private readonly IAzureBlobStoreClient _azureBlobStoreClient;
        private readonly ILogger<FileContentMetadataRepository>? _logger;

        private readonly string _blobContainerName;

        public FileContentMetadataRepository(IAzureBlobStoreClient azureBlobStoreClient, IOptionsSnapshot<AzurePlatformConfiguration> azurePlatformConfiguration, ILogger<FileContentMetadataRepository>? logger)
        {
            _logger = logger;

            _azureBlobStoreClient = azureBlobStoreClient ?? throw new ArgumentNullException(nameof(azureBlobStoreClient));

            if (azurePlatformConfiguration?.Value is null) throw new ArgumentNullException(nameof(azurePlatformConfiguration));

            var blobContainerName = azurePlatformConfiguration.Value.AzureBlobStorage?.ContainerName;

            if (string.IsNullOrWhiteSpace(blobContainerName)) throw new ApplicationException("The files blob container name is not set in the configuration");

            _blobContainerName = blobContainerName;
        }

        async Task<FileContentMetadata> IFileContentMetadataRepository.GetDetailsAndPutContentIntoStreamAsync(UserFileMetadata fileMetadata, Stream streamToWriteTo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (fileMetadata is null) throw new ArgumentNullException(nameof(fileMetadata));

            Debug.Assert(!string.IsNullOrWhiteSpace(fileMetadata.BlobName));
            Debug.Assert(fileMetadata.ContentHash is not null);

            if (streamToWriteTo is null) throw new ArgumentNullException(nameof(streamToWriteTo));

            Debug.Assert(streamToWriteTo.CanWrite);

            var downloadDetails = await _azureBlobStoreClient.FetchBlobAndWriteToStream(_blobContainerName, fileMetadata.BlobName, fileMetadata.FileVersion, fileMetadata.ContentHash, streamToWriteTo, cancellationToken);

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
