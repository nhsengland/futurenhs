using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IImageCommand
{
    Task<Guid> CreateImageAsync(ImageDto image, CancellationToken cancellationToken = default);
    Task ForceDeleteImageAsync(Guid id, CancellationToken cancellationToken = default);
}