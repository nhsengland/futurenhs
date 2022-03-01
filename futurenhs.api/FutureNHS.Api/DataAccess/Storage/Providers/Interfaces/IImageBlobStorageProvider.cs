namespace FutureNHS.Api.DataAccess.Storage.Providers.Interfaces
{
    public interface IImageBlobStorageProvider
    {
        Task UploadFileAsync(Stream stream, string blobName, string contentType,
            CancellationToken cancellationToken);

        Task DeleteFileAsync(string blobName);
        Task CreateConnectionAsync();
    }
}
