using FileServer.Models;

namespace FileServer.Wopi.Interfaces;

public interface IWopiFileContentService
{
    Task<FileContentMetadata> GetFileContentAsync(Models.File file, AuthenticatedUser authenticatedUser, Stream responseStream,
        CancellationToken cancellationToken);

    Task<AzureBlobMetadata> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken);
}