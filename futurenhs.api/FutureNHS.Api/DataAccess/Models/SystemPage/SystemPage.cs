namespace FutureNHS.Api.DataAccess.Models.SystemPage
{
    public sealed record SystemPage : BaseData
    {
        public Guid Id { get; init; }
        public string Slug { get; init; }
        public string Title { get; init; }
        public string Content { get; init; }
    }
}
