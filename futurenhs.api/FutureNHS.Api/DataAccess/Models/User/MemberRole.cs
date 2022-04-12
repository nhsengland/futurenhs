namespace FutureNHS.Api.DataAccess.Models.User
{
    public sealed record MemberRole : BaseData
    {
        public Guid MembershipUserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
