namespace FutureNHS.Api.Services.Interfaces;

public interface ILikeService
{
    Task LikeEntityAsync(Guid membershipUserId, string slug, Guid entityId, CancellationToken cancellationToken);
    Task UnlikeEntityAsync(Guid membershipUserId, string slug, Guid entityId, CancellationToken cancellationToken);
}

