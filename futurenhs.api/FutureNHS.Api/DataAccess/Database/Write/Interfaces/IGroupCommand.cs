using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IGroupCommand
{
    Task<GroupUserDto> GetGroupUserAsync(Guid membershipUserId, Guid groupId, CancellationToken cancellationToken = default);
    Task UserJoinGroupAsync(GroupUserDto groupUser, CancellationToken cancellationToken = default);
    Task UserLeaveGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
    Task<Guid?> GetGroupIdForSlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<GroupData> GetGroupAsync(string slug, CancellationToken cancellationToken = default);
    Task<Guid> CreateGroupAsync(Guid userId, GroupDto groupDto, CancellationToken cancellationToken);
    Task UpdateGroupAsync(GroupDto groupDto, CancellationToken cancellationToken = default);
    Task CreateGroupSiteAsync(GroupSiteDto groupContentDto, CancellationToken cancellationToken);
    Task DeleteGroupSiteAsync(Guid contentId, CancellationToken cancellationToken);
    Task<IEnumerable<GroupRoleDto>> GetAllGroupRolesAsync(CancellationToken cancellationToken = default);
    Task UpdateUserGroupRolesAsync(Guid groupUserId, Guid groupId, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task DeleteUserFromGroupAsync(Guid groupUserId, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task ApproveGroupUserAsync(GroupUserDto groupUserDto, CancellationToken cancellationToken = default);
    Task RejectGroupUserAsync(Guid groupUserId, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task DeleteGroupInviteAsync(Guid groupInviteId, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task<GroupInvite> GetGroupInviteByIdAsync(Guid groupInviteId, CancellationToken cancellationToken = default);

    Task<GroupInvite> GetGroupInviteForUserIdAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
    Task RedeemGroupInviteAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);

    Task<IEnumerable<GroupInvite>> GetGroupInvitesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<(uint totalCount, IEnumerable<PendingGroupMember>)> GetPendingGroupMembersAsync(Guid groupId, uint offset, uint limit, CancellationToken cancellationToken = default);

}
