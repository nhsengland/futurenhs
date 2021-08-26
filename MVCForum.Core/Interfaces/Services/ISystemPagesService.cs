using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvcForum.Core.Models.FilesAndFolders;
using MvcForum.Core.Models.SystemPages;
using MvcForum.Core.Repositories.Models;
using MvcForum.Web.ViewModels.Folder;

namespace MvcForum.Core.Interfaces.Services
{
    public interface ISystemPagesService
    {
        Task<SystemPageWriteResponse> CreateSystemPage(SystemPageWriteViewModel model, CancellationToken cancellationToken);
        Task<SystemPageWriteResponse> UpdateSystemPage(SystemPageWriteViewModel model, CancellationToken cancellationToken);
        Task<SystemPageWriteResponse> DeleteSystemPage(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<SystemPageViewModel>> GetAllSystemPages(CancellationToken cancellationToken);
        Task<SystemPageViewModel> GetSystemPageById(Guid id, CancellationToken cancellationToken);
        Task<SystemPageViewModel> GetSystemPageBySlug(string slug, CancellationToken cancellationToken);
        bool IsValidSlug(string slug);
    }
}
