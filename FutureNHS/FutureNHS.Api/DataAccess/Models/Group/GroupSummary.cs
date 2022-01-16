namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record GroupSummary
    {
        public Guid Id { get; init; }
        public string NameText { get; init; }
        public string StrapLineText { get; init; }
        public int MemberCount { get; init; }
        public int DiscussionCount { get; init; }
        public string Slug { get; init; }
        public ImageData Image { get; init; }
    }
}
