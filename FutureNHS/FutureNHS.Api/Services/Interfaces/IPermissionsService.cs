using System.Security.Claims;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IPermissionsService
    {
        Task<IEnumerable<string>> GetUserPermissionsForGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
