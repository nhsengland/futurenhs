namespace FutureNHS.Api.Models.Identity.Response
{
    public sealed class MemberIdentityResponse
    {
        public Guid MembershipUserId { get; init; }
        public Guid? IdentityId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
