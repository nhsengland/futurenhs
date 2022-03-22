namespace FutureNHS.Api.Services.Interfaces;

public interface ILikeService
{
    Task LikeEntityAsync(Guid membershupUserId, Guid Id, CancellationToken cancellationToken);
    Task UnlikeEntityAsync(Guid membershupUserId, Guid Id, CancellationToken cancellationToken);
}

