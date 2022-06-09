namespace FutureNHS.Api.Models.Identity.Request
{
    public sealed class MemberIdentityRequest
    {
        public Guid IdentityId { get; init; }
        public string EmailAddress { get; init; }
    }
}
