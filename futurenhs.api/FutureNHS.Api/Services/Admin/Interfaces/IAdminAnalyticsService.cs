using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.Services.Admin.Interfaces
{
    public interface IAdminAnalyticsService
    {
        Task<ActiveUsers> GetActiveUsersAsync(Guid adminUserId, DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken);

    }
}
