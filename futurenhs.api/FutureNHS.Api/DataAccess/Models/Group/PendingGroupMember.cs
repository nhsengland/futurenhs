using FutureNHS.Api.DataAccess.Models.Registration;

namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record PendingGroupMember
    {
        public PendingGroupMember() {}

        public PendingGroupMember(Guid userId, string emailAddress, GroupInvite groupInvite, PlatformInvite platformInvite)
        {
            Id = userId;
            Email = emailAddress;
            GroupInvite = groupInvite;
            PlatformInvite = platformInvite;

        }

            
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public GroupInvite? GroupInvite { get; set; }
        public PlatformInvite? PlatformInvite { get; set; }

    }
}