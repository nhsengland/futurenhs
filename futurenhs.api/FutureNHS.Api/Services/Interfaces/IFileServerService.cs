using System.Net;
using FutureNHS.Api.Models.FileServer;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IFileServerService
    {
        Task<FileServerCollaboraResponse> GetCollaboraFileUrl(Guid userId, string slug, Guid file, CookieContainer cookies, string permission, CancellationToken cancellationToken);
    }
}
