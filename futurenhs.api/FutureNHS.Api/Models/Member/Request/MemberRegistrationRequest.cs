namespace FutureNHS.Api.Models.Member.Request
{
    public sealed class MemberRegistrationRequest
    {
        public string Subject { get; init; }
        public string? Issuer { get; init; }
        public string FirstName { get; init; }
        public string? LastName { get; init; }
        public string Email { get; init; }
        public string? JobRole { get; init; }
        public string? Organisation { get; init; }
        public bool Agreed { get; init; }
    }
}
