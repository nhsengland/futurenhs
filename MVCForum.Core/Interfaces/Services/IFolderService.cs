namespace MvcForum.Core.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Web.ViewModels.Folder;

    public interface IFolderService
    {
        FolderViewModel GetFolder(string slug, Guid? folderId);
        FolderReadViewModel GetFolder(Guid? folderId, string folderName, Guid? parentFolder);
        Guid CreateFolder(FolderWriteViewModel model);
        void UpdateFolder(FolderWriteViewModel model);
        bool UserIsAdmin(string groupSlug, Guid? userId);
        IEnumerable<BreadCrumbItem> GenerateBreadcrumbTrail(Guid folderId);
    }
}
