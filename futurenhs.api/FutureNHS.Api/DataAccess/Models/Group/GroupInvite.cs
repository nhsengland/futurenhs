namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record GroupInvite
    {
        public Guid Id { get; set; }
        public Guid MembershipUser_Id { get; set; }
        public Guid GroupId { get; set; } }
}
