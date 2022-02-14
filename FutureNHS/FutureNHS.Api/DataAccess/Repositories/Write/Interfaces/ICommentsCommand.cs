using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface ICommentsCommand
    {
        Task CreateCommentAsync(PostDto post, CancellationToken cancellationToken = default);
    }
}
