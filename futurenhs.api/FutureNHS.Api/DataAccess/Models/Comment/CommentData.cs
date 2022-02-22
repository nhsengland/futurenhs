namespace FutureNHS.Api.DataAccess.Models.Comment
{
    public sealed class CommentData : BaseData
    {
        public Guid Id { get; init; }
        public string Content { get; init; }
        public bool CreatedByThisUser { get; init; }
        public string? CreatedAtUtc { get; init; }
        public Guid CreatedById { get; init; }
        public string CreatedByName { get; init; }
        public string CreatedBySlug { get; init; }
        public int RepliesCount { get; init; }
        public int Likes { get; init; }
        public bool LikedByThisUser { get; init; }
    }
}
