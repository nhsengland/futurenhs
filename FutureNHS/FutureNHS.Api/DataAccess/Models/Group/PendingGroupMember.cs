namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record PendingGroupMember
    {
        public Guid Id { get; init; }
        public string Slug{ get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string ApplicationDateUtc { get; init; }
        public string LastLoginUtc { get; init; }
    }
}
