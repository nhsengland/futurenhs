namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IHealthCheckDataProvider
    {
        Task<bool> CheckDatabaseConnectionAsync(CancellationToken cancellationToken = default);
    }
}
