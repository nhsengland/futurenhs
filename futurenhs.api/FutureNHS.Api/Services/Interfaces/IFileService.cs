using FutureNHS.Api.DataAccess.Models.FileAndFolder;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IFileService
    {
        Task UploadFileMultipartDocument(Guid userId, string slug, Guid folderId, Stream requestBody,
            string? contentType, CancellationToken cancellationToken);

        Task<string> GetFileDownloadUrl(Guid userId, string slug, Guid fileId, CancellationToken cancellationToken);

        Task<AuthUser> CheckUserAccess(Guid userId, Guid fileId, string permission, CancellationToken cancellationToken);
    }
}
