namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IAnalyticsDataProvider
    {
        Task<int> GetActiveUserCountAsync(DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken = default);
    }
}
