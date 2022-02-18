using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface ICommentCommand
    {
        Task CreateCommentAsync(CommentDto comment, CancellationToken cancellationToken = default);
        Task<Guid?> GetThreadIdForComment(Guid commentId, CancellationToken cancellationToken);
    }
}
