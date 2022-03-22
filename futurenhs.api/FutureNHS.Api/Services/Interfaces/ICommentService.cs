using FutureNHS.Api.Models.Comment;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task CreateCommentAsync(Guid userId, string slug, Guid entityId, Comment comment, CancellationToken cancellationToken);
        Task CreateCommentReplyAsync(Guid userId, string slug, Guid entityId, Guid replyingToComment, Comment comment, CancellationToken cancellationToken);
        Task UpdateCommentAsync(Guid userId, string slug, Guid entityId, Guid commentId, Comment comment, byte[] rowVersion, CancellationToken cancellationToken);
        Task DeleteCommentAsync(Guid userId, string slug, Guid entityId, Guid commentId, byte[] rowVersion, CancellationToken cancellationToken);
    }
}
