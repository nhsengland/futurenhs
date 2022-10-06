using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.Registration
{
    public class InviteDetails
    {
        public Guid Id { get; init; }
        public UserNavProperty InvitedBy { get; init; }
        public InviteGroupSummary Group { get; init; }
    }
}
