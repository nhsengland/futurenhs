using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services.Admin
{
    public sealed class AdminAnalyticsService : IAdminAnalyticsService
    {
        private const string AdminViewRole = $"https://schema.collaborate.future.nhs.uk/admin/v1/view";
        
        private readonly ILogger<AdminAnalyticsService> _logger;
        private readonly IAnalyticsDataProvider _analyticsDataProvider;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;

        public AdminAnalyticsService(ILogger<AdminAnalyticsService> logger,
            ISystemClock systemClock,
            IPermissionsService permissionsService, 
            IAnalyticsDataProvider analyticsDataProvider)
        {
            _permissionsService = permissionsService;
            _analyticsDataProvider = analyticsDataProvider;
            _systemClock = systemClock;
            _logger = logger;
        }
        public async Task<ActiveUsers> GetActiveUsersAsync(Guid adminUserId, DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, AdminViewRole, cancellationToken);
            
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetFeatureStatusSelfRegisterAsync - User:{0} does not have access to view admin", adminUserId);
                throw new SecurityException($"Error: User does not have access");
            }
            
            var dailyStartTime = _systemClock.UtcNow.UtcDateTime.AddDays(-1);
            var weeklyStartTime = _systemClock.UtcNow.UtcDateTime.AddDays(-7);
            var monthlyStartTime = _systemClock.UtcNow.UtcDateTime.AddMonths(-1);
            endTime = _systemClock.UtcNow.UtcDateTime.AddMinutes(1);
            
            var dailyActiveUsers = await _analyticsDataProvider.GetActiveUserCountAsync(dailyStartTime, endTime, cancellationToken);
            var weeklyActiveUsers = await _analyticsDataProvider.GetActiveUserCountAsync(weeklyStartTime, endTime, cancellationToken);
            var monthlyActiveUsers = await _analyticsDataProvider.GetActiveUserCountAsync(monthlyStartTime, endTime, cancellationToken);

            var activeUsers = new ActiveUsers()
            {
                Daily = dailyActiveUsers,
                Weekly = weeklyActiveUsers,
                Monthly = monthlyActiveUsers,
            };
            return activeUsers;
        }
    }
}
