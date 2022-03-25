using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IFileCommand
    {
        Task<FileDto> GetFileAsync(Guid id, string status, CancellationToken cancellationToken);
        Task CreateFileAsync(FileDto file, CancellationToken cancellationToken);
        Task<Guid> GetFileStatus(string fileStatus, CancellationToken cancellationToken);
    }
}
