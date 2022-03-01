using System.Security.Claims;
using FutureNHS.Api.Models.Comment;
using FutureNHS.Api.Models.Discussion;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task CreateCommentAsync(Guid userId, string slug, Guid discussionId, Comment comment, CancellationToken cancellationToken);
        Task CreateCommentReplyAsync(Guid userId, string slug, Guid discussionId, Guid replyingToComment, Comment comment, CancellationToken cancellationToken);
        Task UpdateCommentAsync(Guid userId, string slug, Guid discussionId, Guid commentId, Comment comment, byte[] rowVersion, CancellationToken cancellationToken);
        Task DeleteCommentAsync(Guid userId, string slug, Guid discussionId, Guid commentId, byte[] rowVersion, CancellationToken cancellationToken);
    }
}
