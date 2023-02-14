namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed class GroupInviteDto
    {
        public Guid Id { get; set; }
        public Guid MembershipUser_Id { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        
        public Guid CreatedBy { get; set; }
        public DateTime? ExpiresAtUTC { get; set; }
        public bool IsDeleted { get; init; }
        public byte[] RowVersion { get; set; }
    }
}
