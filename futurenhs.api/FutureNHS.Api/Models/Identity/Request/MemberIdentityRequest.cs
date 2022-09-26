namespace FutureNHS.Api.Models.Identity.Request
{
    public sealed class MemberIdentityRequest
    {
        public string SubjectId { get; init; }
        public string EmailAddress { get; init; }
    }
}
