namespace FutureNHS.Api.DataAccess.Models.Identity
{
    public sealed class Identity
    {
        public Guid MembershipUserId { get; init; }
        public string SubjectId { get; init; }
        public string Issuer { get; init; }
    }
}
