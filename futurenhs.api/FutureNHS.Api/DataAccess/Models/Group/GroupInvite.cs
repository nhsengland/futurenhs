namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record GroupInvite
    {
        public GroupInvite() {}

        public GroupInvite(GroupInvite invite)
        {
            Id = invite.Id;
            MembershipUser_Id = invite.MembershipUser_Id;
            CreatedAtUTC = invite.CreatedAtUTC;
            GroupId = invite.GroupId;
            RowVersion = invite.RowVersion;
        }

            
        public Guid Id { get; set; }
        public Guid MembershipUser_Id { get; set; }
        public Guid GroupId { get; set; }
        public DateTime CreatedAtUTC { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime? ExpiresAtUTC { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
