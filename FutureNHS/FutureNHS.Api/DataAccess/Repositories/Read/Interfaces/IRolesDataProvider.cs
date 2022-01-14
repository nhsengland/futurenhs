using FutureNHS.Api.DataAccess.Models.Permissions;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface IRolesDataProvider
    {
        Task<UserAndGroupRoles> GetUserAndGroupUserRolesAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>?> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
