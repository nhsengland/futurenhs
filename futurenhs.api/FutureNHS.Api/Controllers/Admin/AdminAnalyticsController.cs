using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class AdminAnalyticsController : ControllerIdentityBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IAdminAnalyticsService _adminAnalyticsService;
        private readonly IUserService _userService;
        private readonly IEtagService _etagService;

        public AdminAnalyticsController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService, ILogger<UsersController> logger, 
            IPermissionsService permissionsService,
            IAdminAnalyticsService adminAnalyticsService,
            IEtagService etagService) : base(baseLogger, userService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _adminAnalyticsService = adminAnalyticsService;
            _userService = userService;
            _etagService = etagService;
        }
        
        [HttpGet]
        [Route("admin/analytics")]
        public async Task<ActiveUsers> GetActiveUserCountAsync(DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken = default)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var analytics = await _adminAnalyticsService.GetActiveUsersAsync(identity.MembershipUserId, startTime,
                endTime, cancellationToken);

            return analytics;
        }
    }
}