using FutureNHS.Api.DataAccess.Models.Discussions;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IDiscussionDataProvider
    {
        Task<uint> GetDiscussionCountForGroupAsync(string groupSlug, CancellationToken cancellationToken);
        Task<IEnumerable<Discussion>> GetDiscussionsForGroupAsync(Guid? userId, string groupSlug,
            uint offset, uint limit, CancellationToken cancellationToken);

        Task<Discussion?> GetDiscussionAsync(Guid? userId, string groupSlug, Guid id,
            CancellationToken cancellationToken);
        Task<DiscussionCreatorDetails> GetDiscussionCreatorDetailsAsync(Guid discussionId, CancellationToken cancellationToken);
    }
}
