using FutureNHS.Api.DataAccess.Models.SystemPage;

namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface ISystemPageDataProvider
    {
        Task<SystemPage> GetSystemPageAsync(string systemPageSlug, CancellationToken cancellationToken);
    }
}
