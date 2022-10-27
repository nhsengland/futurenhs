using FutureNHS.Api.DataAccess.Models.Search;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface ISearchDataProvider
    {
        Task<(uint totalCount, SearchResults)> Search(Guid userId, string term, uint offset, uint limit, CancellationToken cancellationToken);
    }
}
