using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IPermissionsService
    {
        Task<UserGroupPermissions> GetUserPermissionsForGroupAsync(Guid userId, Guid groupId, string memberStatus, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> UserCanPerformActionAsync(Guid userId, string action, CancellationToken cancellationToken);
        Task<bool> UserCanPerformActionAsync(Guid userId, Guid groupId, string action, CancellationToken cancellationToken);
        Task<bool> UserCanPerformActionAsync(Guid userId, string slug, string action, CancellationToken cancellationToken);
    }
}
