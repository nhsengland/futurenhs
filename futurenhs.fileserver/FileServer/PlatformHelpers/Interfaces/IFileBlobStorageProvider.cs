using Azure.Storage.Blobs.Models;


namespace FileServer.PlatformHelpers.Interfaces
{
    public interface IAzureBlobStoreClient
    {
        Task<BlobDownloadDetails> FetchBlobAndWriteToStream(string containerName, string blobName, string? blobVersion,Stream steamToWriteTo, byte[] contentHash, CancellationToken cancellationToken);
        Task<Uri> GenerateEphemeralDownloadLink(string containerName, string blobName, string blobVersion, string publicFacingBlobName, CancellationToken cancellationToken);
    }
}
