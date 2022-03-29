using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IFolderCommand
    {
        Task<FolderDto> GetFolderAsync(Guid folderId, CancellationToken cancellationToken);
        Task<Guid> CreateFolderAsync(Guid userId, Guid groupId, FolderDto folder, CancellationToken cancellationToken);
        Task UpdateFolderAsync(Guid userId, FolderDto folder, byte[] rowVersion, CancellationToken cancellationToken);
        Task<bool> IsFolderUniqueAsync(string folderTitle, Guid? folderId, Guid groupId, CancellationToken cancellationToken);
    }
}
