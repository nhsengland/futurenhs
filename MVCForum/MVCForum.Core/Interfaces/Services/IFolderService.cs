namespace MvcForum.Core.Interfaces.Services
{
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Web.ViewModels.Folder;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IFolderService
    {
        Task<FolderViewModel> GetFolderAsync(string slug, Guid? folderId, CancellationToken cancellationToken);
        Task<bool> IsFolderNameValidAsync(Guid? folderId, string folderName, Guid? parentFolderId, Guid parentGroupId, CancellationToken cancellationToken);
        Task<bool> IsFolderIdValidAsync(Guid folderId, CancellationToken cancellationToken);
        Task<IEnumerable<BreadCrumbItem>> GenerateBreadcrumbTrailAsync(Guid folderId, CancellationToken cancellationToken);
        Task<bool> IsUserAdminAsync(string groupSlug, Guid userId, CancellationToken cancellationToken);
        Task<bool> UserHasFolderReadAccessAsync(string groupSlug, Guid userId, CancellationToken cancellationToken);
        Task<bool> UserHasFolderWriteAccessAsync(Guid folderId, Guid userId, CancellationToken cancellationToken);
        Task<bool> UserHasFileWriteAccessAsync(Guid folderId, Guid userId, CancellationToken cancellationToken);
        Guid CreateFolder(FolderWriteViewModel model);
        void UpdateFolder(FolderWriteViewModel model);
        Task<bool> DeleteFolderAsync(Guid folderId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
