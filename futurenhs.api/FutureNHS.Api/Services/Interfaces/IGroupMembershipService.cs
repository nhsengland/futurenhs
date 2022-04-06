using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IGroupMembershipService
    {
        Task UserJoinGroupAsync(Guid userId, string slug, CancellationToken cancellationToken);
        Task UserLeaveGroupAsync(Guid userId, string slug, CancellationToken cancellationToken);
        Task<GroupMemberDetails> GetGroupMembershipUserAsync(Guid userId, Guid groupUserId, string slug, CancellationToken cancellationToken);
        Task<IEnumerable<GroupRoleDto>> GetMembershipRolesForGroupAsync(Guid userId, string slug, CancellationToken cancellationToken); 
        Task UpdateGroupMembershipUserRoleAsync(Guid userId, string slug, Guid groupUserId, Guid groupRoleId, byte[] rowVersion, CancellationToken cancellationToken);
        Task DeleteGroupMembershipUserAsync(Guid userId, string slug, Guid groupUserId, byte[] rowVersion, CancellationToken cancellationToken);
    }
}
