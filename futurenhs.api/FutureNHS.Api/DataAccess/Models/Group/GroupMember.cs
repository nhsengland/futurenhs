namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record GroupMember : BaseData
    {
        public Guid Id { get; init; }
        public Guid GroupUserId { get; init; }
        public string Slug{ get; init; }
        public string Name { get; init; }
        public DateTime DateJoinedUtc { get; init; }
        public DateTime LastLoginUtc { get; init; }
        public string Role { get; init; }
    }
}
