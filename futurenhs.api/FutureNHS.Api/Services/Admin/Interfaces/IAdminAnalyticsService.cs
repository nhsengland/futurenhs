using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Member;
using FutureNHS.Application.Application;

namespace FutureNHS.Api.Services.Admin.Interfaces
{
    public interface IAdminAnalyticsService
    {
        Task<int> GetActiveUsersAsync(Guid adminUserId, DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken);

    }
}
