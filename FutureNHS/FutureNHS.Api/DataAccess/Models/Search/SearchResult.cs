namespace FutureNHS.Api.DataAccess.Models.Search
{
    public record SearchResult
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? LastUpdatedAtUtc { get; set; }
        public GroupNavProperty Group { get; set; }
    }
}
