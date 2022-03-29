using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface ILikeCommand
{
    Task CreateLikedEntityAsync(EntityLikeDto entityLike, CancellationToken cancellationToken);
    Task DeleteLikedEntityAsync(EntityLikeDto entityLike, CancellationToken cancellationToken);
}

