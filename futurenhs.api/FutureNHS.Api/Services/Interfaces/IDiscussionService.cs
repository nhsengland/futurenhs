using FutureNHS.Api.Models.Discussion;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IDiscussionService
    {
        Task CreateDiscussionAsync(Guid userId, string slug, Discussion discussion, CancellationToken cancellationToken);

        Task<IEnumerable<DataAccess.Models.Discussions.Discussion>> GetDiscussionsForGroupAsync(Guid? userId,
            string slug,
            uint offset, uint limit, string? sortBy, CancellationToken cancellationToken);
    }
}
