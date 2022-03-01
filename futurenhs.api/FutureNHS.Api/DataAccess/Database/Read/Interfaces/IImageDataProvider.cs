using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IImageDataProvider
    {
        Task<Image> GetImageAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
