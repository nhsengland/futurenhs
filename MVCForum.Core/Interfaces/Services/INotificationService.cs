namespace MvcForum.Core.Interfaces.Services
{
    using System.Collections.Generic;
    using Models.Entities;
    using Models.Enums;

    public partial interface INotificationService : IContextService
    {
        #region Groups

        void Delete(GroupNotification notification);
        List<GroupNotification> GetGroupNotificationsByGroup(Group Group);
        List<GroupNotification> GetGroupNotificationsByUser(MembershipUser user);
        List<GroupNotification> GetGroupNotificationsByUserAndGroup(MembershipUser user, Group Group, bool addTracking = false);
        GroupNotification Add(GroupNotification Group);

        #endregion

        #region Tags

        void Delete(TagNotification notification);
        IList<TagNotification> GetTagNotificationsByTag(TopicTag tag);
        IList<TagNotification> GetTagNotificationsByTag(List<TopicTag> tag);
        IList<TagNotification> GetTagNotificationsByUser(MembershipUser user);
        IList<TagNotification> GetTagNotificationsByUserAndTag(MembershipUser user, TopicTag tag, bool addTracking = false);
        TagNotification Add(TagNotification tagNotification);

        #endregion

        #region Topic

        void Delete(TopicNotification notification);
        IList<TopicNotification> GetTopicNotificationsByTopic(Topic topic);
        IList<TopicNotification> GetTopicNotificationsByUser(MembershipUser user);
        IList<TopicNotification> GetTopicNotificationsByUserAndTopic(MembershipUser user, Topic topic, bool addTracking = false);
        TopicNotification Add(TopicNotification topicNotification);
        #endregion

        void Notify(Topic topic, MembershipUser loggedOnReadOnlyUser, NotificationType notificationType);
    }
}