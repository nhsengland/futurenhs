using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Models.FilesAndFolders;
using MvcForum.Core.Repositories.Models;
using MvcForum.Web.ViewModels.Folder;

namespace MvcForum.Core.Interfaces.Services
{
    public interface IFolderService
    {
        FolderViewModel GetFolder(string slug, Guid? folderId);

        Guid CreateFolder(FolderWriteViewModel model);
        void UpdateFolder(FolderWriteViewModel model);
        bool UserIsAdmin(string groupSlug, Guid? userId);
        IEnumerable<BreadCrumbItem> GenerateBreadcrumbTrail(Guid folderId);
    }
}
