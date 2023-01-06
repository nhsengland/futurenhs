using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface ILikeDataProvider
    {
        Task<EntityLikeData> GetEntityLikesAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<EntityLikeData>> GetEntityLikeListAsync(Guid entityId, CancellationToken cancellationToken = default);
    }
}
