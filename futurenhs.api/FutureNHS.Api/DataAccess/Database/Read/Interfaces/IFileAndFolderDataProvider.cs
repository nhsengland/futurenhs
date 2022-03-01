using FutureNHS.Api.DataAccess.Models.FileAndFolder;
using File = FutureNHS.Api.DataAccess.Models.FileAndFolder.File;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IFileAndFolderDataProvider
    {
        Task<(uint total, IEnumerable<FolderContentsItem>?)> GetRootFoldersAsync(string groupSlug, uint offset, uint limit, CancellationToken cancellationToken);
        Task<Folder?> GetFolderAsync(Guid folderId, CancellationToken cancellationToken);
        Task<File?> GetFileAsync(Guid fileId, CancellationToken cancellationToken);
        Task<(uint total,IEnumerable<FolderContentsItem>?)> GetFolderContentsAsync(Guid folderId, uint offset, uint limit, CancellationToken cancellationToken);
    }
}
