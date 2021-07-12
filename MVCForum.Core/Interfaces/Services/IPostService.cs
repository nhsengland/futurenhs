namespace MvcForum.Core.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Models.Entities;
    using Models.Enums;
    using Models.General;
    using Pipeline;

    public partial interface IPostService : IContextService
    {
        Post SanitizePost(Post post);
        Post GetTopicStarterPost(Guid topicId);
        IEnumerable<Post> GetAll(List<Group> allowedGroups);
        IList<Post> GetLowestVotedPost(int amountToTake);
        IList<Post> GetHighestVotedPost(int amountToTake);
        IList<Post> GetByMember(Guid memberId, int amountToTake, List<Group> allowedGroups);
        IList<Post> GetReplyToPosts(Post post);
        IList<Post> GetReplyToPosts(Guid postId);
        IEnumerable<Post> GetPostsByFavouriteCount(Guid postsByMemberId, int minAmountOfFavourites);
        IEnumerable<Post> GetPostsFavouritedByOtherMembers(Guid postsByMemberId);

        Task<PaginatedList<Post>> SearchPosts(int pageIndex, int pageSize, int amountToTake, string searchTerm,
            List<Group> allowedGroups);

        Task<PaginatedList<Post>> GetPagedPostsByTopic(int pageIndex, int pageSize, int amountToTake, Guid topicId,
            PostOrderBy order);
        IList<Post> GetPostsByThread(Guid threadId);
        Task<PaginatedList<Post>> GetPagedPostsByThread(int pageIndex, int pageSize, int amountToTake,
            Guid threadId, PostOrderBy order);
        Post GetLatestPostForThread(Guid threadId, PostOrderBy order);
        int GetPostCountForThread(Guid threadId);
        int TopicPostCount(Guid topicId);

        Task<PaginatedList<Post>> GetPagedPendingPosts(int pageIndex, int pageSize, List<Group> allowedGroups);
        IList<Post> GetPendingPosts(List<Group> allowedGroups, MembershipRole usersRole);
        int GetPendingPostsCount(List<Group> allowedGroups);

        Task<IPipelineProcess<Post>> Create(string postContent, Topic topic, MembershipUser user, HttpPostedFileBase[] files, bool isTopicStarter, Guid? replyTo);
        Task<IPipelineProcess<Post>> Create(Post post, HttpPostedFileBase[] files, bool isTopicStarter, Guid? replyTo);
        Task<IPipelineProcess<Post>> Edit(Post post, HttpPostedFileBase[] files, bool isTopicStarter, string postedTopicName, string postedContent);
        Task<IPipelineProcess<Post>> Move(Post post, Guid? newTopicId, string newTopicTitle, bool moveReplyToPosts);

        Post Initialise(string postContent, Topic topic, MembershipUser user);
        Post Get(Guid postId);
        IList<Post> GetPostsByTopics(List<Guid> topicIds, List<Group> allowedGroups);
        Task<IPipelineProcess<Post>> Delete(Post post, bool ignoreLastPost);
        IList<Post> GetSolutionsByMember(Guid memberId, List<Group> allowedGroups);
        int PostCount(List<Group> allowedGroups);
        IList<Post> GetPostsByMember(Guid memberId, List<Group> allowedGroups);
        IList<Post> GetAllSolutionPosts(List<Group> allowedGroups);
        IList<Post> GetPostsByTopic(Guid topicId);
        IEnumerable<Post> GetAllWithTopics(List<Group> allowedGroups);
        bool PassedPostFloodTest(MembershipUser user);
    }
}