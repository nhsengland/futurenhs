namespace FutureNHS.Api.DataAccess.Models
{
    public sealed record EntityLikeData
    {
        public Guid Id { get; init; }

        public bool CreatedByThisUser { get; init; }
        public DateTime? CreatedAtUtc { get; init; }
        
        public Shared.Properties FirstRegistered { get; init; }
    }
}
