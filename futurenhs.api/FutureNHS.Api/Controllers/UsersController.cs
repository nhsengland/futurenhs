using System.Net.Mail;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Models.UserInvite;
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
        private readonly IUserDataProvider _userDataProvider;
        private readonly IUserService _userService;

        public UsersController(ILogger<GroupsController> logger, IPermissionsService permissionsService, IUserDataProvider userDataProvider,IUserService userService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _userDataProvider = userDataProvider;
            _userService = userService;

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
        [Route("users/{userId:guid}/admin/users")]
        public async Task<IActionResult> GetMembersAsync(Guid userId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (total, members) = await _userService.GetMembersAsync(userId, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(members, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("users/{userId:guid}/admin/invite")]
        public async Task<IActionResult> InviteMemberToGroupAndPlatformAsync(Guid userId, UserInvite userInvite, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(userInvite.EmailAddress))
                throw new ArgumentNullException(nameof(userInvite.EmailAddress));

            await _userService.InviteMemberToGroupAndPlatformAsync(userId, userInvite.GroupId, userInvite.EmailAddress, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{id:guid}")]
        public async Task<IActionResult> GetMemberAsync(Guid id, CancellationToken cancellationToken)
        {
            var member = await _userDataProvider.GetMemberAsync(id, cancellationToken);

            if (member is null)
                return NotFound();

            return Ok(member);
        }
    }
}