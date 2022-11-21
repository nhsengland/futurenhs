using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IAnalyticsDataProvider
    {
        Task<ActiveUsers> GetActiveUserCountAsync(DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken = default);
    }
}
