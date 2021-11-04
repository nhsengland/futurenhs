namespace MvcForum.Core.ExtensionMethods
{
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.Enums;

    public static class GroupUserExtensions
    {
        public static GroupUserStatus GetUserStatusForGroup(this GroupUser user)
        {
            if (user == null)
            {
                return GroupUserStatus.NotJoined;
            }

            if (user.Approved && !user.Banned && !user.Locked)
            {
                return GroupUserStatus.Joined;
            }

            if (!user.Approved && !user.Banned && !user.Locked && !user.Rejected)
            {
                return GroupUserStatus.Pending;
            }

            if (user.Approved && user.Banned && !user.Locked)
            {
                return GroupUserStatus.Banned;
            }

            if (user.Approved && !user.Banned && user.Locked)
            {
                return GroupUserStatus.Locked;
            }

            return user.Rejected ? GroupUserStatus.Rejected : GroupUserStatus.NotJoined;
        }
    }
}
