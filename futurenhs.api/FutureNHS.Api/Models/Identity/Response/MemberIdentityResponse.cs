using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.Models.Identity.Response
{
    public sealed record MemberIdentityResponse
    {
        public Guid MembershipUserId { get; init; }
        public Guid? IdentityId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public ImageData Image { get; init; }
    }
}
