using FutureNHS.Api.Models.FileServer;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IFileServerService
    {
        Task<FileServerCollaboraResponse?> GetCollaboraFileUrl(Guid userId, string slug, string permission, Guid file, string authHeader, CancellationToken cancellationToken);
    }
}
