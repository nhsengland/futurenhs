using FutureNHS.Api.Attributes;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class UsersController : ControllerIdentityBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IUserService _userService;
        private readonly IEtagService _etagService;
  
        public UsersController(ILogger<ControllerIdentityBase> baseLogger, ILogger<UsersController> logger, IPermissionsService permissionsService, IUserService userService, IEtagService etagService, IGroupMembershipService groupMembershipService) : base(baseLogger, userService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _userService = userService;
            _etagService = etagService;
        }

        [HttpGet]
        [Route("users/actions")]
        public async Task<IActionResult> GetActionsUserCanPerformInGroupAsync(CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var permissions = await _permissionsService.GetUserPermissionsAsync(identity.MembershipUserId, cancellationToken);

            if (permissions is null)
            {
                return NotFound();
            }

            return Ok(permissions);
        }

        [HttpGet]
        [Route("users/{targetUserId:guid}")]
        public async Task<IActionResult> GetMemberAsync(Guid targetUserId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var member = await _userService.GetMemberProfileAsync(identity.MembershipUserId, targetUserId, cancellationToken);

            if (member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpGet]
        [Route("users/{targetUserId:guid}/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetMemberForUpdateAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken)
        { 
           var identity = await GetUserIdentityAsync(cancellationToken);
           var member = await _userService.GetMemberAsync(identity.MembershipUserId, targetUserId, cancellationToken);

           if (member is null)
               return NotFound();

           return Ok(member);
        }

        [HttpPut]
        [DisableFormValueModelBinding]
        [Route("users/{targetUserId:guid}/update")]
        public async Task<IActionResult> UpdateMemberAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken)
        {
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }

            var rowVersion = _etagService.GetIfMatch();

            var identity = await GetUserIdentityAsync(cancellationToken);
            await _userService.UpdateMemberAsync(identity.MembershipUserId, targetUserId, Request.Body, Request.ContentType, rowVersion, cancellationToken);

            return Ok();
        }
        
        [HttpPost]
        [Route("users/info")]
        public async Task<IActionResult> MemberInfoAsync(CancellationToken cancellationToken)
        {
            var subjectId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(subjectId))
                throw new UnauthorizedAccessException();
            
            var emailAddress = User.FindFirst("emails")?.Value;       
            var memberInfoResponse = await _userService.GetMemberInfoAsync(subjectId, emailAddress, cancellationToken);

            return Ok(memberInfoResponse);
        }
    }
}