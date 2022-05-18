namespace FutureNHS.Api.DataAccess.Models.Comment
{
    public class CommentCreatorDetails
    {
        public Guid CommentId { get; init; }
        public Guid DiscussionId { get; init; }
        public string GroupSlug { get; init; }
        public string? CreatedAtUtc { get; init; }
        public Guid CreatedById { get; init; }
        public string CreatedByName { get; init; }
        public string CreatedByEmail { get; init; }
        public string CreatedBySlug { get; init; }
    }
}
