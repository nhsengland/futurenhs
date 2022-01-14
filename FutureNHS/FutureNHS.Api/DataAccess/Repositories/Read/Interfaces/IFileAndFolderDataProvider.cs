using FutureNHS.Api.DataAccess.Models.FileAndFolder;
using FutureNHS.Api.DataAccess.Models.Permissions;
using File = FutureNHS.Api.DataAccess.Models.FileAndFolder.File;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface IFileAndFolderDataProvider
    {
        Task<Folder?> GetFolderAsync(Guid folderId, CancellationToken cancellationToken);
        Task<File?> GetFileAsync(Guid fileId, CancellationToken cancellationToken);
        Task<(uint total,IEnumerable<FolderContentsItem>?)> GetFolderContentsAsync(Guid folderId, uint offset, uint limit, CancellationToken cancellationToken);
    }
}
