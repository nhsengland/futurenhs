using FutureNHS.Api.DataAccess.Models.Discussions;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface IDiscussionDataProvider
    {
        Task<(uint total, IEnumerable<Discussion>?)> GetDiscussionsForGroupAsync(Guid? userId, string groupSlug,
            uint offset, uint limit, CancellationToken cancellationToken);

        Task<Discussion?> GetDiscussionAsync(Guid? userId, string groupSlug, Guid id,
            CancellationToken cancellationToken);
    }
}
