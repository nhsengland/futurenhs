using FutureNHS.Api.Models.UserInvite;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRegistrationService _registrationService;

        public RegistrationController(ILogger<RegistrationController> logger, IPermissionsService permissionsService, IRegistrationService registrationService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _registrationService = registrationService;

        }



        [HttpPost]
        [Route("users/{userId:guid}/registration/invite")]
        public async Task<IActionResult> InviteMemberToGroupAndPlatformAsync(Guid userId,
            [FromBody] UserInvite userInvite, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userInvite.EmailAddress))
                throw new ArgumentNullException(nameof(userInvite.EmailAddress));

            await _registrationService.InviteMemberToGroupAndPlatformAsync(userId, userInvite.GroupSlug,
                userInvite.EmailAddress, cancellationToken);

            return Ok();
        }
    }
}