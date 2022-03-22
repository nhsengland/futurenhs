using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record EntityLikeDto : BaseData
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Guid MembershipUserId { get; set; }
        public DateTime? CreatedAtUTC { get; set; }
    }
}
