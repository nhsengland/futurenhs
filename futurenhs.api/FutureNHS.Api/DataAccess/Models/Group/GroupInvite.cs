namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record GroupInvite
    {
        public Guid Id { get; set; }
        public Guid MembershipUser_Id { get; set; }
        public Guid GroupId { get; set; }
        public DateTime CreatedAtUTC { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime? ExpiresAtUTC { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
