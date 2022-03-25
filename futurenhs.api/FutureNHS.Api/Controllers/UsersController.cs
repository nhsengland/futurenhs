using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class UsersController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly IPermissionsService _permissionsService;

        public UsersController(ILogger<GroupsController> logger, IPermissionsService permissionsService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
        }

        [HttpGet]
        [Route("users/{userId:guid}/actions")]
        public async Task<IActionResult> GetActionsUserCanPerformInGroupAsync(Guid userId, CancellationToken cancellationToken)
        {
            var permissions = await _permissionsService.GetUserPermissionsAsync(userId, cancellationToken);
           
            if (permissions is null)
            {
                return NotFound();
            }

            return Ok(permissions);
        }
    }
}