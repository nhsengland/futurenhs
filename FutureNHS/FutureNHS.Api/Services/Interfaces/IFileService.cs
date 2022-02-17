using FutureNHS.Api.Models.File;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IFileService
    {
        Task CreateFileAsync(Guid userId, string slug, Guid folderId, FutureNHS.Api.Models.File.File file, CancellationToken cancellationToken);
    }
}
