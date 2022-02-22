using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Comment;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface ICommentCommand
    {
        Task<CommentData> GetCommentAsync(Guid commentId, CancellationToken cancellationToken);
        Task CreateCommentAsync(CommentDto comment, CancellationToken cancellationToken = default);
        Task UpdateCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken = default);
        Task<Guid?> GetThreadIdForComment(Guid commentId, CancellationToken cancellationToken);
    }
}
