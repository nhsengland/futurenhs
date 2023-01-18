using Azure.Storage.Sas;
using FutureNHS.Api.Models.File;
using Microsoft.Azure.Storage.Blob;

namespace FutureNHS.Api.DataAccess.Storage.Providers.Interfaces
{
    public interface IFileBlobStorageProvider
    {
        Task<AzureBlobMetaData?> UploadFileAsync(Stream stream, string blobName, string contentType,
            CancellationToken cancellationToken);

        Task DeleteFileAsync(string blobName);

        string GetRelativeDownloadUrl(string blobName, string fileName, SharedAccessBlobPermissions downloadPermissions,
            CancellationToken cancellationToken);
    }
}
