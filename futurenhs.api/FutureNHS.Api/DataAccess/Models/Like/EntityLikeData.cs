namespace FutureNHS.Api.DataAccess.Models
{
    public sealed record EntityLikeData : BaseData
    {
        public Guid EntityId { get; init; }
        public Guid MembershipUserId { get; init; }
        public bool CreatedByThisUser { get; init; }
        public string? CreatedAtUtc { get; init; }
    }
}
