namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record PendingGroupMember
    {
        public Guid Id { get; init; }
        public string Slug{ get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public DateTime ApplicationDateUtc { get; init; }
        public DateTime LastLoginUtc { get; init; }
    }
}
