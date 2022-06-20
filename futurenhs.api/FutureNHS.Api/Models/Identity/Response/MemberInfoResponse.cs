using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.Models.Identity.Enums;

namespace FutureNHS.Api.Models.Identity.Response
{
    public sealed record MemberInfoResponse
    {
        public Guid MembershipUserId { get; set; }
        public string SubjectId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public ImageData? Image { get; set; }
    }
}
