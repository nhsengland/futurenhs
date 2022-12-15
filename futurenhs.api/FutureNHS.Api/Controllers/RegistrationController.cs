using FutureNHS.Api.Attributes;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.Models.Domain.Request;
using FutureNHS.Api.Models.Identity.Request;
using FutureNHS.Api.Models.Member.Request;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Models.UserInvite;
using FutureNHS.Api.Services;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class RegistrationController : ControllerIdentityBase
    {
        private readonly string? _defaultGroup;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRegistrationService _registrationService;
        private readonly IGroupMembershipService _groupMembershipService;
        private readonly IUserService _userService;
        private readonly IEtagService _etagService;

        public RegistrationController(ILogger<ControllerIdentityBase> baseLogger, ILogger<RegistrationController> logger, IEtagService etagService, IPermissionsService permissionsService,
            IUserService userService, IGroupMembershipService groupMembershipService, IRegistrationService registrationService, 
            IOptionsMonitor<DefaultSettings> defaultSettings) : base(baseLogger, userService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _registrationService = registrationService;
            _defaultGroup = defaultSettings.CurrentValue.DefaultGroup;
            _groupMembershipService = groupMembershipService;
            _userService = userService;
            _etagService = etagService;
        }

        [HttpPost]
        [Route("groups/{slug}/registration/invite")]
        public async Task<IActionResult> InviteMemberToGroupAndPlatformAsync(Guid userId,
            [FromBody] UserInvite userInvite, string slug, CancellationToken cancellationToken)
        {
            //CHECK EMAIL DOMAIN
            if (string.IsNullOrEmpty(userInvite.EmailAddress))
                throw new ArgumentNullException(nameof(userInvite.EmailAddress));
        
            var identity = await GetUserIdentityAsync(cancellationToken);
            await _registrationService.InviteMemberToGroupAndPlatformAsync(identity.MembershipUserId, slug,
                userInvite.EmailAddress, cancellationToken);
        
            return Ok();
        }
        
        [HttpPost]
        [Route("registration/invite")]
        public async Task<IActionResult> InviteMemberToPlatformAsync(Guid userId,
            [FromBody] UserInvite userInvite, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userInvite.EmailAddress))
                throw new ArgumentNullException(nameof(userInvite.EmailAddress));

            var identity = await GetUserIdentityAsync(cancellationToken);
            await _registrationService.InviteMemberToPlatformAsync(identity.MembershipUserId,
                userInvite.EmailAddress, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("registration/domains/{domainId}")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetDomainAsync(Guid userId, Guid domainId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var domain = await _registrationService.GetDomainAsync(identity.MembershipUserId, domainId, cancellationToken);
            if (domain is null)
                return NotFound();

            return Ok(domain);
        }

        [HttpPut]
        [Route("registration/domains/{domainId}")]
        public async Task<IActionResult> DeleteDomainAsync(Guid userId, Guid domainId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var rowVersion = _etagService.GetIfMatch();
            await _registrationService.DeleteDomainAsync(identity.MembershipUserId, domainId, rowVersion, cancellationToken);

            return Ok();
        }
        
        [HttpPost]
        [Route("registration/domains")]
        public async Task<IActionResult> AddDomainAsync(Guid userId, RegisterDomainRequest registerDomainRequest, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            await _registrationService.AddDomainAsync(identity.MembershipUserId, registerDomainRequest, cancellationToken);

            return Ok();
        }
        
        [HttpGet]
        [Route("registration/domains")]
        public async Task<IActionResult> GetDomainsAsync(Guid userId, Guid adminUserId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var route = Request.Path.Value;

            var (total, domains) = await _registrationService.GetDomainsAsync(identity.MembershipUserId, filter.Offset, filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(domains, filter, total, route);

            return Ok(pagedResponse);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("registration/invite/{id:guid}")]
        public async Task<IActionResult> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.NewGuid())
                throw new ArgumentNullException(nameof(id));

            var invite = await _registrationService.GetRegistrationInviteDetailsAsync(id, cancellationToken);
            return Ok(invite);
        }
        
        [HttpPut]
        [Route("registration/invite/{inviteId}")]
        public async Task<IActionResult> DeleteDomainAsync(Guid inviteId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var rowVersion = _etagService.GetIfMatch();
            await _registrationService.DeletePlatformInviteAsync(identity.MembershipUserId, inviteId, rowVersion, cancellationToken);

            return Ok();
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

            var isMemberInvited = await _userService.CheckMemberCanRegisterAsync(emailAddress, cancellationToken);
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

        [HttpPost]
        [Route("registration/identity")]
        public async Task<IActionResult> AddMemberIdentityAsync(MemberIdentityRequest memberIdentityRequest, CancellationToken cancellationToken)
        {
            var response = await _registrationService.MapMemberToIdentityAsync(memberIdentityRequest, cancellationToken);

            return Ok(response);

        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("registration/public/exists")]
        public async Task<IActionResult> GetSelfRegistrationEnabledAsync(CancellationToken cancellationToken)
        {

            var isEnabled = await _registrationService.GetSelfRegistrationEnabledAsync();
            return Ok(isEnabled);
        }

        
    }
}