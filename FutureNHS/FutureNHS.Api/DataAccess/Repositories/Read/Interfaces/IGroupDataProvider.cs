using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Infrastructure.Models.GroupPages;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface IGroupDataProvider
    {
        Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid id, uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> DiscoverGroupsForUserAsync(Guid id, uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<Group> GetGroupAsync(string slug, CancellationToken cancellationToken = default);
    }
}
