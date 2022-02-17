using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface IFolderCommand
    {
        Task CreateFolderAsync(Guid userId, Guid groupId, FolderDto folder, CancellationToken cancellationToken);
    }
}
