namespace FutureNHS.Api.DataAccess.Models
{
    public sealed record EntityLikeData
    {
        public Guid EntityId { get; init; }
        public Guid MembershipUserId { get; init; }
        public bool CreatedByThisUser { get; init; }
        public DateTime? CreatedAtUtc { get; init; }
        
        public Shared.Properties FirstRegistered { get; init; }
    }
}
