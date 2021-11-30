namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Models;

    public interface IFolderRepository
    {
        Task<FolderReadViewModel> GetFolderAsync(Guid folderId, CancellationToken cancellationToken);
        Task<bool> IsFolderNameValidAsync(string folderName, Guid? parentFolderId, Guid parentGroupId, CancellationToken cancellationToken);
        Task<bool> IsFolderIdValidAsync(Guid folderId, CancellationToken cancellationToken);
        Task<PaginatedList<FolderReadViewModel>> GetRootFoldersForGroupAsync(string groupSlug, int page = 1, int pageSize = 999, CancellationToken cancellationToken = default(CancellationToken));
        Task<PaginatedList<FolderReadViewModel>> GetChildFoldersForFolderAsync(Guid parentFolderId, int page = 1, int pageSize = 999, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsUserAdminAsync(string groupSlug, Guid userId, CancellationToken cancellationToken);
        Task<bool> UserHasGroupAccessAsync(string groupSlug, Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<BreadCrumbItem>> GenerateBreadcrumbTrailAsync(Guid folderId, CancellationToken cancellationToken);
    }
}
