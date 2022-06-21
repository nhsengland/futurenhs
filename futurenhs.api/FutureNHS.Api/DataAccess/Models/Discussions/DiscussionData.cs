namespace FutureNHS.Api.DataAccess.Models.Discussions
{
    public record DiscussionData
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public bool CreatedByThisUser { get; init; }
        public string? CreatedAtUtc { get; init; }
        public Guid CreatedById { get; init; }
        public string CreatedByName { get; init; }
        public string CreatedBySlug { get; init; }
        public Guid? LastComment { get; init; }
        public string? LastCommentAtUtc { get; init; }
        public Guid? LastCommenterId { get; init; }
        public string? LastCommenterName { get; init; }
        public string? LastCommenterSlug { get; init; }
        public bool IsSticky { get; init; }
        public int Views { get; init; }
        public int? TotalComments { get; init; }
        public ImageData? Image { get; init; }
    }
}
