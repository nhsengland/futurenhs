using FutureNHS.Application.Application.HardCodedSettings;
using FutureNHS.Infrastructure.Models;

namespace FutureNHS.Infrastructure.Repositories.Read.Interfaces
{
    public interface IHealthCheckDataProvider
    {
        Task<bool> CheckDatabaseConnectionAsync(CancellationToken cancellationToken = default);
    }
}
