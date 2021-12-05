using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.GroupPages;
using FutureNHS.Application.Application.HardCodedSettings;
using FutureNHS.Infrastructure.Models;
using FutureNHS.Infrastructure.Models.GroupPages;

namespace FutureNHS.Infrastructure.Repositories.Read.Interfaces
{
    public interface IGroupDataProvider
    {
        Task<(int totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid id, int page = PaginationSettings.MinPageNumber, int pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<(int totalGroups, IEnumerable<GroupSummary> groupSummaries)> DiscoverGroupsForUserAsync(Guid id, int page = PaginationSettings.MinPageNumber, int pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<GroupHeader> GetGroupHeaderForUserAsync(Guid userId, string slug, CancellationToken cancellationToken = default);
        Task<GroupHomePage> GetGroupHomePage(string slug, CancellationToken cancellationToken = default);


    }
}
