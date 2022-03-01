using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IFileCommand
    {
        Task CreateFileAsync(FileDto file, CancellationToken cancellationToken);
        Task<Guid> GetFileStatus(string fileStatus, CancellationToken cancellationToken);
    }
}
