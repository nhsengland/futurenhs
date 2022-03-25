namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime CreatedAtUTC { get; init; }
        public Guid? ModifiedBy { get; init; }
        public DateTime? ModifiedAtUTC { get; init; }
        public bool FlaggedAsSpam { get; init; }
        public Guid? InReplyTo { get; init; }
        public Guid DiscussionId { get; init; }
        public Guid? ThreadId { get; init; }
        public bool IsDeleted { get; init; }
        public byte[] RowVersion { get; set; }
    }
}
