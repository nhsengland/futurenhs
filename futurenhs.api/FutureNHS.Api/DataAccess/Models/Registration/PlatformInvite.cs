namespace FutureNHS.Api.DataAccess.Models.Registration
{
    public sealed record PlatformInvite
    {
        public PlatformInvite() {}

        public PlatformInvite(PlatformInvite invite)
        {
            Id = invite.Id;
            CreatedAtUTC = invite.CreatedAtUTC;
            Email = invite.Email;
            GroupId = invite.GroupId;
            RowVersion = invite.RowVersion;
        }

            
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public DateTime CreatedAtUTC { get; set; }

        public Guid CreatedBy { get; set; }
        public string Email { get; set; }
        
        public DateTime? ExpiresAtUTC { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
