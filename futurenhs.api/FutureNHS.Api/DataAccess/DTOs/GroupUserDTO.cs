namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed class GroupUserDto
    {
        public Guid Id { get; set; }
        public bool Approved { get; set; }
        public bool Rejected { get; set; }
        public bool Locked { get; set; }
        public bool Banned { get; set; }
        public DateTime? RequestToJoinDateUTC { get; set; }
        public DateTime? ApprovedDateUTC { get; set; }
        public string RequestToJoinReason { get; set; }
        public string LockReason { get; set; }
        public string BanReason { get; set; }
        public Guid? ApprovingMembershipUser { get; set; }
        public Guid MembershipRole { get; set; }
        public Guid MembershipUser { get; set; }
        public Guid Group { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
