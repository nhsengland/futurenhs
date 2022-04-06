namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record GroupMember : BaseData
    {
        public Guid Id { get; init; }
        public Guid GroupUserId { get; init; }
        public string Slug{ get; init; }
        public string Name { get; init; }
        public string DateJoinedUtc { get; init; }
        public string LastLoginUtc { get; init; }
        public string Role { get; init; }
    }
}
