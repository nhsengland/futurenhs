namespace FutureNHS.Api.Models.Member
{
    public record MemberRoleUpdate
    {
        public Guid MembershipUserId { get; set; }
        public Guid CurrentRoleId { get; set; }
        public Guid NewRoleId { get; set; }
    }
}
