using FutureNHS.Application.Application.HardCodedSettings;
using FutureNHS.Infrastructure.Models;

namespace FutureNHS.Infrastructure.Repositories.Read.Interfaces
{
    public interface IImageDataProvider
    {
        Task<Image> GetImageAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
