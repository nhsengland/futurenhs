using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Models.Collabora;

namespace MvcForum.Core.Interfaces.Services
{
    public interface IFileServerService
    {
        Task<FileServerResponse> GetCollaboraFileUrl(Guid file, CookieContainer cookies);
    }
}
