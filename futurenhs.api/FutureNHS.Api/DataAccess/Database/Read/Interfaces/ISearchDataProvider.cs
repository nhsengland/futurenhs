using FutureNHS.Api.DataAccess.Models.Search;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface ISearchDataProvider
    {
        Task<(uint totalCount, SearchResults)> Search(string term, uint offset, uint limit, CancellationToken cancellationToken);
    }
}
