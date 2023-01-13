using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.FileAndFolder;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IFileCommand
    {
        Task<FileDto> GetFileAsync(Guid id, string status, CancellationToken cancellationToken);
        Task CreateFileAsync(FileDto file, CancellationToken cancellationToken);
        Task<Guid> GetFileStatus(string fileStatus, CancellationToken cancellationToken);
        Task<AuthUserData?> GetFileAccess(Guid userId, Guid fileId, CancellationToken cancellationToken);
        Task<AuthUserData> GetFileVersionAccess(Guid userId, Guid fileId, CancellationToken cancellationToken);
    }
}
