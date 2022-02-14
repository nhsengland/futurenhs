namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record PostDto
    {
        public string PostContent { get; init; }
        public DateTime DateCreated { get; init; }
        public DateTime DateEdited { get; init; }
        public bool IsTopicStarter { get; init; }
        public bool FlaggedAsSpam { get; init; }
        public bool Pending { get; init; }
        public Guid? InReplyTo { get; init; }
        public Guid TopicId { get; init; }
        public Guid MembershipUserId { get; init; }
    }
}
