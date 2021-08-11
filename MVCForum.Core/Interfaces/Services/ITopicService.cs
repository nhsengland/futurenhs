namespace MvcForum.Core.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Models.Entities;
    using Models.General;
    using Pipeline;

    public partial interface ITopicService : IContextService
    {
        IList<Topic> GetAll(List<Group> allowedGroups);
        IList<SelectListItem> GetAllSelectList(List<Group> allowedGroups, int amount);
        IList<Topic> GetHighestViewedTopics(int amountToTake, List<Group> allowedGroups);

        IList<Topic> GetPopularTopics(DateTime? from, DateTime? to, List<Group> allowedGroups,
            int amountToShow = 20);

        Task<IPipelineProcess<Topic>> Create(Topic topic, HttpPostedFileBase[] files, string tags, bool subscribe, string postContent, Post post);

        Task<IPipelineProcess<Topic>> Edit(Topic topic, HttpPostedFileBase[] files, string tags, bool subscribe, string postContent, string originalTopicName, List<PollAnswer> pollAnswers, int closePollAfterDays);

        IList<Topic> GetTodaysTopics(int amountToTake, List<Group> allowedGroups);

        Task<PaginatedList<Topic>> GetRecentTopics(int pageIndex, int pageSize, int amountToTake,
            List<Group> allowedGroups);

        IList<Topic> GetRecentRssTopics(int amountToTake, List<Group> allowedGroups);
        IList<Topic> GetTopicsByUser(Guid memberId, List<Group> allowedGroups);
        IList<Topic> GetTopicsByLastPost(List<Guid> postIds, List<Group> allowedGroups);
        Task<PaginatedList<Topic>> GetPagedTopicsByGroupAsync(int pageIndex, int pageSize, int amountToTake, Guid GroupId);
        PaginatedList<Topic> GetPagedTopicsByGroup(int pageIndex, int pageSize, int amountToTake, Guid GroupId);
        Task<PaginatedList<Topic>> GetPagedPendingTopics(int pageIndex, int pageSize, List<Group> allowedGroups);
        IList<Topic> GetPendingTopics(List<Group> allowedGroups, MembershipRole usersRole);
        int GetPendingTopicsCount(List<Group> allowedGroups);
        IList<Topic> GetRssTopicsByGroup(int amountToTake, Guid GroupId);

        Task<PaginatedList<Topic>> GetPagedTopicsByTag(int pageIndex, int pageSize, int amountToTake, string tag,
            List<Group> allowedGroups);

        IList<Topic> SearchTopics(int amountToTake, string searchTerm, List<Group> allowedGroups);

        Task<PaginatedList<Topic>> GetTopicsByCsv(int pageIndex, int pageSize, int amountToTake, List<Guid> topicIds,
            List<Group> allowedGroups);

        Task<PaginatedList<Topic>> GetMembersActivity(int pageIndex, int pageSize, int amountToTake, Guid memberGuid,
            List<Group> allowedGroups);

        IList<Topic> GetTopicsByCsv(int amountToTake, List<Guid> topicIds, List<Group> allowedGroups);
        IList<Topic> GetSolvedTopicsByMember(Guid memberId, List<Group> allowedGroups);
        Topic GetTopicBySlug(string slug);
        Topic Get(Guid topicId);
        List<Topic> Get(List<Guid> topicIds, List<Group> allowedGroups);
        Task<IPipelineProcess<Topic>> Delete(Topic topic);
        int TopicCount(List<Group> allowedGroups);

        List<MarkAsSolutionReminder> GetMarkAsSolutionReminderList(int days);

        /// <summary>
        ///     Mark a topic as solved
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="post"></param>
        /// <param name="marker"></param>
        /// <param name="solutionWriter"></param>
        /// <returns>True if topic has been marked as solved</returns>
        Task<bool> SolveTopic(Topic topic, Post post, MembershipUser marker, MembershipUser solutionWriter);

        IList<Topic> GetAllTopicsByGroup(Guid GroupId);

        Task<PaginatedList<Topic>> GetPagedTopicsAll(int pageIndex, int pageSize, int amountToTake,
            List<Group> allowedGroups);

        IList<Topic> GetTopicBySlugLike(string slug);
    }
}