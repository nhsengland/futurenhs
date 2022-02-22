using FutureNHS.Api.DataAccess.Models.Comment;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface ICommentsDataProvider
    {
        Task<(uint total, IEnumerable<Comment>?)> GetCommentsForDiscussionAsync(Guid? userId, string groupSlug, Guid topicId, uint offset, uint limit, CancellationToken cancellationToken);
        Task<(uint total, IEnumerable<Comment>?)> GetRepliesForCommentAsync(Guid? userId, string groupSlug, Guid threadId, uint offset, uint limit, CancellationToken cancellationToken);
    }
}
