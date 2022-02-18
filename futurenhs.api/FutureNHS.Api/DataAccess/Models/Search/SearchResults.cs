namespace FutureNHS.Api.DataAccess.Models.Search
{
    public record SearchResults
    {
        public IEnumerable<SearchResult>? Results { get; init; }
        public SearchResultTotalsByType? TotalsByType { get; init; }
    }
}