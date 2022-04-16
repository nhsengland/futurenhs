namespace FutureNHS.Api.DataAccess.Models.User
{
    public record MemberSearchDetails
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Initials { get; init; }
        public string Email { get; init; }
        public string Username { get; init; }
        public string Role { get; init; }
    }
}
