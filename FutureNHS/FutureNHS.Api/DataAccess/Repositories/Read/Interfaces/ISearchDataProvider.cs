using FutureNHS.Api.DataAccess.Models.Search;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface ISearchDataProvider
    {
        Task<(uint totalCount, SearchResults)> Search(string term, uint offset, uint limit, CancellationToken cancellationToken);
    }
}
