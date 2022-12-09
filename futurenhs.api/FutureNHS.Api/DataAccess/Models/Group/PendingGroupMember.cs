namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record PendingGroupMember
    {
        public PendingGroupMember() {}

        public PendingGroupMember(Guid userId, string emailAddress, GroupInvite invite)
        {
            Id = userId;
            Email = emailAddress;
            Invite = invite;
        }

            
        public Guid Id { get; set; }
        public string Email { get; set; }
        public GroupInvite? Invite { get; set; }
    }
}