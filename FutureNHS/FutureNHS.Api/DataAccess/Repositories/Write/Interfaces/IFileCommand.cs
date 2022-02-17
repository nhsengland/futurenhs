using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface IFileCommand
    {
        Task CreateFileAsync(FileDto file, CancellationToken cancellationToken);
        Task<Guid> GetFileStatus(string fileStatus, CancellationToken cancellationToken);
    }
}
