namespace FutureNHS.Api.DataAccess.Models.Discussions
{
    public record DiscussionCreatorDetails
    {
        public Guid DiscussionId { get; init; }
        public string GroupSlug { get; init; }
        public DateTime? CreatedAtUtc { get; init; }
        public Guid CreatedById { get; init; }
        public string CreatedByName { get; init; }
        public string CreatedByEmail { get; init; }
        public string CreatedBySlug { get; init; }
    }
}
