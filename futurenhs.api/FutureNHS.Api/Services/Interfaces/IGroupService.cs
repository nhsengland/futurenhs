using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IGroupService
    {
        Task<(uint totalGroups, IEnumerable<AdminGroupSummary> groupSummaries)> AdminGetGroupsAsync(Guid userId,
            uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize,
            CancellationToken cancellationToken = default);

        Task<GroupData?> GetGroupAsync(Guid userId, string slug, CancellationToken cancellationToken);

        Task UpdateGroupMultipartDocument(Guid userId, string slug, byte[] rowVersion, Stream requestBody, string? contentType,
            CancellationToken cancellationToken);
        Task<(uint, IEnumerable<GroupMember>)> GetGroupMembersAsync(Guid userId, string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken);
        Task<(uint, IEnumerable<PendingGroupMember>)> GetPendingGroupMembersAsync(Guid userId, string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken);
        Task<GroupSite> CreateGroupSiteDataAsync(Guid userId, string slug, CancellationToken cancellationToken);
        Task<Group?> GetGroupAsync(string slug, Guid userId, CancellationToken cancellationToken);
        Task<GroupMemberDetails> GetGroupMemberAsync(Guid userId, string slug, Guid memberId, CancellationToken cancellationToken);
        Task<GroupSite> GetGroupSiteDataAsync(Guid userId, string groupSlug, CancellationToken cancellationToken);
        Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid userId, bool isMember, uint offset, uint limit, CancellationToken cancellationToken);
        
        Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetPendingGroupsForUserAsync(Guid userId, bool isMember, uint offset, uint limit, CancellationToken cancellationToken);

    }
}