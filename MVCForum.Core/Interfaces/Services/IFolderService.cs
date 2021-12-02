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
        Task<bool> IsFolderNameValidAsync(string folderName, Guid? parentFolderId, Guid parentGroupId, CancellationToken cancellationToken);
        Task<bool> IsFolderIdValidAsync(Guid folderId, CancellationToken cancellationToken);
        Task<bool> IsUserAdminAsync(string groupSlug, Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<BreadCrumbItem>> GenerateBreadcrumbTrailAsync(Guid folderId, CancellationToken cancellationToken);
        Task<bool> UserHasGroupAccessAsync(string groupSlug, Guid userId, CancellationToken cancellationToken);
        Task<bool> UserHasFileAccessAsync(Guid fileId, Guid userId, CancellationToken cancellationToken);
        Guid CreateFolder(FolderWriteViewModel model);
        void UpdateFolder(FolderWriteViewModel model);
        /// <summary>
        /// Delete a folder  by id - returns boolean to confirm deletion.
        /// </summary>
        /// <param name="folderId"><see cref="Guid"/> - Id of the folder.</param>
        /// <returns>Boolean -true if success</returns>
        Task<bool> DeleteFolderAsync(Guid folderId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
