using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Member;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Security;
using FutureNHS.Application.Application;
using Microsoft.FeatureManagement;

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
            
            startTime = _systemClock.UtcNow.UtcDateTime.AddDays(-1);
            endTime = _systemClock.UtcNow.UtcDateTime;
            var activeUsers = await _analyticsDataProvider.GetActiveUserCountAsync(startTime, endTime, cancellationToken);

            return activeUsers;
        }
    }
}
