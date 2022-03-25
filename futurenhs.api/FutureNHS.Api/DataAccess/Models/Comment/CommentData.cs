namespace FutureNHS.Api.DataAccess.Models.Comment
{
    public sealed record CommentData : BaseData
    {
        public Guid Id { get; init; }
        public string Content { get; init; }
        public bool CreatedByThisUser { get; init; }
        public string? CreatedAtUtc { get; init; }
        public Guid CreatedById { get; init; }
        public string CreatedByName { get; init; }
        public string CreatedBySlug { get; init; }
        public Guid? InReplyTo { get; init; }
        public int RepliesCount { get; init; }
        public int Likes { get; init; }
        public bool LikedByThisUser { get; init; }
    }
}
