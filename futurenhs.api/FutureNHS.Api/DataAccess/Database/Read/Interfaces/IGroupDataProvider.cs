using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IGroupDataProvider
    {
        Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid id, uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> DiscoverGroupsForUserAsync(Guid id, uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<(uint totalGroups, IEnumerable<AdminGroupSummary> groupSummaries)> AdminGetGroupsAsync(uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<Group?> GetGroupAsync(string slug, Guid userId, CancellationToken cancellationToken = default);
        Task<(uint, IEnumerable<GroupMember>)> GetGroupMembersAsync(string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken = default);
        Task<(uint, IEnumerable<PendingGroupMember>)> GetPendingGroupMembersAsync(string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken = default);
        Task<GroupMemberDetails?> GetGroupMemberAsync(string slug, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<GroupMemberDetails>> GetGroupAdminsAsync(string groupSlug, CancellationToken cancellationToken = default);
        Task<GroupSite> GetGroupSiteDataAsync(Guid groupId, CancellationToken cancellationToken);
    }
}
