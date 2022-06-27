using FutureNHS.Api.DataAccess.Models.Comment;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface ICommentsDataProvider
    {
        Task<uint> GetCommentsCountForDiscussionAsync(string groupSlug, Guid discussionId, CancellationToken cancellationToken);
        Task<IEnumerable<Comment>> GetCommentsForDiscussionAsync(Guid? userId, string groupSlug, Guid discussionId, uint offset, uint limit, CancellationToken cancellationToken);
        Task<uint> GetRepliesCountForCommentAsync(string groupSlug, Guid threadId, CancellationToken cancellationToken);
        Task<IEnumerable<Comment>> GetRepliesForCommentAsync(Guid? userId, string groupSlug, Guid threadId, uint offset, uint limit, CancellationToken cancellationToken);
        Task<CommentCreatorDetails> GetCommentCreatorDetailsAsync(Guid commentId, CancellationToken cancellationToken);
    }
}
