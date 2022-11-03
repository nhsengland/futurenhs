using FileServer.Models;

namespace FileServer.DataAccess.Interfaces;

public interface IFileMetaDataProvider
{
    Task<UserFileMetadata> GetFileMetaDataForUserAsync(Guid fileId, Guid userId, CancellationToken cancellationToken);

    Task UpdateFileMetaDataForUserAsync(Guid fileId, Guid userId, byte[] contentHash, DateTime modifiedAtutc, CancellationToken cancellationToken);

}