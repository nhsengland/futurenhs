using Azure.Storage.Blobs.Models;
using FileServer.Models;


namespace FileServer.PlatformHelpers.Interfaces
{
    public interface IAzureBlobStoreClient
    {
        Task<BlobDownloadDetails> FetchBlobAndWriteToStream(string containerName, string blobName, string? blobVersion,Stream steamToWriteTo, byte[] contentHash, CancellationToken cancellationToken);

        Task<AzureBlobMetadata> UploadFileAsync(Stream stream, string blobName, string contentType, CancellationToken cancellationToken);
        Task<Uri> GenerateEphemeralDownloadLink(string containerName, string blobName, string blobVersion, string publicFacingBlobName, CancellationToken cancellationToken);
    }
}
