using System.Security.Claims;
using FutureNHS.Api.Models.Comment;
using FutureNHS.Api.Models.Discussion;
using FutureNHS.Api.Models.Folder;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IFolderService
    {
        Task CreateFolderAsync(Guid userId, string slug, Folder folder, CancellationToken cancellationToken);
        Task CreateChildFolderAsync(Guid userId, string slug, Guid parentFolderId, Folder folder, CancellationToken cancellationToken);
    }
}
