using System.Security.Claims;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IPermissionsService
    {
        Task<IEnumerable<string>?> GetUserPermissionsForGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
        Task<IEnumerable<string>?> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> UserCanPerformActionAsync(Guid userId, Guid groupId, string action, CancellationToken cancellationToken);
        Task<bool> UserCanPerformActionAsync(Guid userId, string slug, string action, CancellationToken cancellationToken);
    }
}
