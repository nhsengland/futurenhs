namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface IHealthCheckDataProvider
    {
        Task<bool> CheckDatabaseConnectionAsync(CancellationToken cancellationToken = default);
    }
}
