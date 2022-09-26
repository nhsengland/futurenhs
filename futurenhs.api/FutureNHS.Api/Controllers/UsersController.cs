using FutureNHS.Api.Attributes;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Models.Identity.Request;
using FutureNHS.Api.Models.Member.Request;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class UsersController : ControllerBase
    {
        private readonly string? _defaultGroup;
        private readonly ILogger<UsersController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IUserService _userService;
        private readonly IEtagService _etagService;
        private readonly IGroupMembershipService _groupMembershipService;
  
        public UsersController(ILogger<UsersController> logger, IPermissionsService permissionsService, IUserDataProvider userDataProvider, IUserService userService, IEtagService etagService, IGroupMembershipService groupMembershipService, IOptionsMonitor<DefaultSettings> defaultSettings)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _userDataProvider = userDataProvider;
            _userService = userService;
            _etagService = etagService;
            _groupMembershipService = groupMembershipService;
            _defaultGroup = defaultSettings.CurrentValue.DefaultGroup;
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

        [HttpGet]
        [Route("users/{userId:guid}/users/{targetUserId:guid}")]
        public async Task<IActionResult> GetMemberAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken)
        {
            var member = await _userDataProvider.GetMemberProfileAsync(targetUserId, cancellationToken);

            if (member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpGet]
        [Route("users/{userId:guid}/users/{targetUserId:guid}/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetMemberForUpdateAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken)
        {        
           var member = await _userService.GetMemberAsync(userId, targetUserId, cancellationToken);

           if (member is null)
               return NotFound();

           return Ok(member);
        }

        [HttpPut]
        [DisableFormValueModelBinding]
        [Route("users/{userId:guid}/users/{targetUserId:guid}/update")]
        public async Task<IActionResult> UpdateMemberAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken)
        {
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }

            var rowVersion = _etagService.GetIfMatch();

            await _userService.UpdateMemberAsync(userId, targetUserId, Request.Body, Request.ContentType, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/register/{emailAddress}")]
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
        [Route("users/register")]
        public async Task<IActionResult> RegisterMemberAsync(MemberRegistrationRequest memberRegistrationRequest, CancellationToken cancellationToken)
        {
           var userId = await _userService.RegisterMemberAsync(memberRegistrationRequest, cancellationToken);

           if (!userId.HasValue) return Forbid();
           
           if (_defaultGroup is not null)
           {
               await _groupMembershipService.UserJoinGroupAsync(userId.Value, _defaultGroup, cancellationToken);
           }

           return Ok(userId);

        }

        [HttpPost]
        [Route("users/info")]
        public async Task<IActionResult> MemberInfoAsync([FromBody] MemberIdentityRequest memberIdentity, CancellationToken cancellationToken)
        {
            var memberInfoResponse = await _userService.GetMemberInfoAsync(memberIdentity, cancellationToken);

            return Ok(memberInfoResponse);
        }
    }
}