namespace FutureNHS.Api.DataAccess.Models.Registration
{
    public sealed record PlatformInvite
    {
        public Guid Id { get; init; }
        public Guid GroupId { get; init; }
        public DateTime CreatedAtUTC { get; init; }

        public Guid CreatedBy { get; init; }
        public string Email { get; init; }
        
        public DateTime? ExpiresAtUTC { get; init; }
        public byte[] RowVersion { get; init; }
    }
}
