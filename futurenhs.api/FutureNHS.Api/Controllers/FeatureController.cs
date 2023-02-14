using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FutureNHS.Api.DataAccess.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;

namespace FutureNHS.Api.Controllers
{   [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class FeatureController : ControllerIdentityBase
    {
        private readonly ILogger<FeatureController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IUserService _userService;
        private readonly IFeatureManager _featureManager;
        private const string AdminViewRole = $"https://schema.collaborate.future.nhs.uk/admin/v1/view";


        public FeatureController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService,
            ILogger<FeatureController> logger,
            IPermissionsService permissionsService, IFeatureManager featureManager) : base(baseLogger, userService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _userService = userService;
            _featureManager = featureManager;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("features/{flag}")]
        public async Task<IActionResult> IsFlagEnabledAsync(string flag, CancellationToken cancellationToken = default)
        {
            bool isFlagEnabled = await _featureManager.IsEnabledAsync(flag);
            return Ok(isFlagEnabled);

        }
        
        [HttpGet]
        [Route("features")]
        public async Task<IActionResult> GetAllFlagsAsync(CancellationToken cancellationToken = default)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(identity.MembershipUserId, AdminViewRole,
                    cancellationToken);
            
            if (userCanPerformAction is false)
                return Forbid();
            
            var flagNames = _featureManager.GetFeatureNamesAsync();
            var flags = new List<FeatureFlag>();
            await foreach (var flag in flagNames)
            {
                var isFlagEnabled = await _featureManager.IsEnabledAsync(flag);
                var featureFlag = new FeatureFlag(flag, isFlagEnabled);
                flags.Add(featureFlag);
            }
            return Ok(flags);
        }
    }
}


    