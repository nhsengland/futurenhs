using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.Models.Member.Request;
using FutureNHS.Api.Models.UserInvite;
using FutureNHS.Api.Services;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class RegistrationController : ControllerBase
    {
        private readonly string? _defaultGroup;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRegistrationService _registrationService;
        private readonly IGroupMembershipService _groupMembershipService;
        private readonly IUserService _userService;

        public RegistrationController(ILogger<RegistrationController> logger, IPermissionsService permissionsService, IUserService userService, IGroupMembershipService groupMembershipService, IRegistrationService registrationService, IOptionsMonitor<DefaultSettings> defaultSettings)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _registrationService = registrationService;
            _defaultGroup = defaultSettings.CurrentValue.DefaultGroup;
            _groupMembershipService = groupMembershipService;
            _userService = userService;


        }



        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/registration/invite")]
        public async Task<IActionResult> InviteMemberToGroupAndPlatformAsync(Guid userId,
            [FromBody] UserInvite userInvite, string slug, CancellationToken cancellationToken)
        {
            //CHECK EMAIL DOMAIN
            if (string.IsNullOrEmpty(userInvite.EmailAddress))
                throw new ArgumentNullException(nameof(userInvite.EmailAddress));
        
            await _registrationService.InviteMemberToGroupAndPlatformAsync(userId, slug,
                userInvite.EmailAddress, cancellationToken);
        
            return Ok();
        }
        
        [HttpPost]
        [Route("users/{userId:guid}/registration/invite")]
        public async Task<IActionResult> InviteMemberToPlatformAsync(Guid userId,
            [FromBody] UserInvite userInvite, CancellationToken cancellationToken)
        {
            //CHECK EMAIL DOMAIN
            if (string.IsNullOrEmpty(userInvite.EmailAddress))
                throw new ArgumentNullException(nameof(userInvite.EmailAddress));

            await _registrationService.InviteMemberToPlatformAsync(userId, userInvite.GroupSlug,
                userInvite.EmailAddress, cancellationToken);

            return Ok();
        }


        [HttpGet]
        [Route("registration/invite/{id:guid}")]
        public async Task<IActionResult> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.NewGuid())
                throw new ArgumentNullException(nameof(id));

            var invite = await _registrationService.GetRegistrationInviteDetailsAsync(id, cancellationToken);
            return Ok(invite);
        }
        
        [HttpGet]
        [Route("registration/register/{emailAddress}")]
        public async Task<IActionResult> RegisterMemberAsync(string emailAddress, CancellationToken cancellationToken)
        {
            var memberDetailsResponse = await _userService.GetMemberByEmailAsync(emailAddress, cancellationToken);
            if (memberDetailsResponse is not null)
            {
                return Ok(memberDetailsResponse);
            }

            var isMemberInvited = await _userService.IsMemberInvitedAsync(emailAddress, cancellationToken);
            if (isMemberInvited)
            {
                return Ok();
            }

            return Forbid();
        }
        
        [HttpPost]
        [Route("registration/register")]
        public async Task<IActionResult> RegisterMemberAsync(MemberRegistrationRequest memberRegistrationRequest, CancellationToken cancellationToken)
        {
            //CHECK EMAIL DOMAIN
            var userId = await _registrationService.RegisterMemberAsync(memberRegistrationRequest, cancellationToken);

            if (!userId.HasValue) return Forbid();
           
            if (_defaultGroup is not null)
            {
                await _groupMembershipService.UserJoinGroupAsync(userId.Value, _defaultGroup, cancellationToken);
            }

            return Ok(userId);

        }


    }
}