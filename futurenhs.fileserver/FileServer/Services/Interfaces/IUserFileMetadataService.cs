using FileServer.Models;
using File = FileServer.Models.File;


namespace FileServer.Services.Interfaces;

public interface IUserFileMetadataService
{
    Task<UserFileMetadata> GetForFileAsync(Guid fileId, AuthenticatedUser authenticatedUser, CancellationToken cancellationToken);
}