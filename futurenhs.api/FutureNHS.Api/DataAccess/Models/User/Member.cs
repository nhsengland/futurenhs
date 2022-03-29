namespace FutureNHS.Api.DataAccess.Models.User
{
    public record Member
    {
        public Guid Id { get; init; }
        public string Slug{ get; init; }
        public string Name { get; init; }
        public string DateJoinedUtc { get; init; }
        public string LastLoginUtc { get; init; }
        public string Role { get; init; }

    }
}
