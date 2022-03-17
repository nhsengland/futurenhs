using FutureNHS.Api.Models.Folder;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IFolderService
    {
        Task<Folder> GetFolderAsync(Guid userId, string slug, Guid folderId, CancellationToken cancellationToken);
        Task<Guid> CreateFolderAsync(Guid userId, string slug, Folder folder, CancellationToken cancellationToken);
        Task<Guid> CreateChildFolderAsync(Guid userId, string slug, Guid parentFolderId, Folder folder, CancellationToken cancellationToken);
        Task UpdateFolderAsync(Guid userId, string slug, Guid folderId, Folder folder, byte[] rowVersion, CancellationToken cancellationToken);
    }
}
