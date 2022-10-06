using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.Registration
{
    public record InviteDetails
    {
        public Guid Id { get; init; }
        public UserNavProperty InvitedBy { get; init; }
        public GroupSummary Group { get; init; }
    }
}
