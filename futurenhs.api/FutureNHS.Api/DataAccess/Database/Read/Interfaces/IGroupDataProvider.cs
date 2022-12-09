using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IGroupDataProvider
    {
        Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid userId, bool isMember, uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        
        Task<(uint totalGroups, IEnumerable<GroupInviteSummary> groupSummaries)> GetGroupInvitesForUserAsync(Guid userId, IEnumerable<GroupInvite> groupInvites,
            uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);

        Task<(uint totalGroups, IEnumerable<AdminGroupSummary> groupSummaries)> AdminGetGroupsAsync(uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<Group?> GetGroupAsync(string slug, Guid userId, CancellationToken cancellationToken = default);
        Task<(uint, IEnumerable<GroupMember>)> GetGroupMembersAsync(string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken = default);
        Task<(uint, IEnumerable<PendingGroupMember>)> GetPendingGroupMembersAsync(IEnumerable<GroupInvite> invitesList, uint offset, uint limit, string sort, CancellationToken cancellationToken = default);
        Task<GroupMemberDetails?> GetGroupMemberAsync(string slug, Guid userId, CancellationToken cancellationToken = default);
        Task<GroupSite> GetGroupSiteDataAsync(string groupSlug, CancellationToken cancellationToken);
        Task<IEnumerable<GroupMemberDetails>> GetGroupAdminsAsync(string groupSlug, CancellationToken cancellationToken = default);
        Task<bool> GetGroupPrivacyStatusAsync(string groupSlug, CancellationToken cancellationToken = default);
        Task<bool> GetGroupPrivacyStatusAsync(Guid GroupId, CancellationToken cancellationToken = default);
    }
}
