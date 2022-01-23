using FutureNHS.Api.DataAccess.Models.Discussions;
using FutureNHS.Api.DataAccess.Models.FileAndFolder;
using FutureNHS.Api.DataAccess.Models.Permissions;
using File = FutureNHS.Api.DataAccess.Models.FileAndFolder.File;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface IDiscussionDataProvider
    {
        Task<(uint total, IEnumerable<Discussion>?)> GetDiscussionsForGroupAsync(string groupSlug, uint offset, uint limit, CancellationToken cancellationToken);
        Task<Discussion?> GetDiscussionAsync(Guid id, string groupSlug, CancellationToken cancellationToken);
    }
}
