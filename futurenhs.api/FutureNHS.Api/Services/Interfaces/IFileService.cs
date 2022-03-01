using FutureNHS.Api.Models.File;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IFileService
    {
        Task UploadFileMultipartDocument(Guid userId, string slug, Guid folderId, Stream requestBody,
            string? contentType, CancellationToken cancellationToken);
    }
}
