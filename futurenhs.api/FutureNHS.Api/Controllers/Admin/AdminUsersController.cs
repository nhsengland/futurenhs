using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Attributes;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Models.Member;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Models.UserInvite;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class AdminUsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IAdminUserService _adminUserService;
        private readonly IUserService _userService;
        private readonly IEtagService _etagService;

        public AdminUsersController(ILogger<UsersController> logger, 
            IPermissionsService permissionsService,
            IAdminUserService adminUserService,
            IUserService userService,
            IEtagService etagService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _adminUserService = adminUserService;
            _userService = userService;
            _etagService = etagService;
        }

        [HttpGet]
        [Route("users/{adminUserId:guid}/admin/users/search")]
        public async Task<IActionResult> SearchMembersAsync(Guid adminUserId, [FromQuery, MinLength(SearchSettings.TermMinimum), MaxLength(SearchSettings.TermMaximum)] string term, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            term = term.Trim();
            var (total, members) = await _adminUserService.SearchMembersAsync(adminUserId, term, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(members, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{adminUserId:guid}/admin/users")]
        public async Task<IActionResult> GetMembersAsync(Guid adminUserId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (total, members) = await _adminUserService.GetMembersAsync(adminUserId, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(members, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("users/{adminUserId:guid}/admin/users/invite")]
        public async Task<IActionResult> InviteMemberToGroupAndPlatformAsync(Guid adminUserId, [FromBody] UserInvite userInvite, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userInvite.EmailAddress))
                throw new ArgumentNullException(nameof(userInvite.EmailAddress));

            await _adminUserService.InviteMemberToGroupAndPlatformAsync(adminUserId, userInvite.GroupId, userInvite.EmailAddress, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{adminUserId:guid}/admin/users/roles")]
        public async Task<IActionResult> GetMembershipRolesAsync(Guid adminUserId, CancellationToken cancellationToken)
        {
            var roles = await _adminUserService.GetMemberRolesAsync(adminUserId, cancellationToken);

            if (roles is null)
                return NotFound();

            return Ok(roles);
        }

        [HttpGet]
        [Route("users/{adminUserId:guid}/admin/users/{userId:guid}/roles")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetMemberForRoleUpdateAsync(Guid adminUserId, Guid userId, CancellationToken cancellationToken)
        {
            var member = await _adminUserService.GetMemberRoleAsync(adminUserId, userId, cancellationToken);

            if (member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpPut]
        [DisableFormValueModelBinding]
        [Route("users/{adminUserId:guid}/admin/users/{userId:guid}/roles")]
        public async Task<IActionResult> UpdateMemberRoleAsync(Guid adminUserId, Guid userId, [FromBody] MemberRoleUpdate memberRoleUpdate, CancellationToken cancellationToken)
        {
            var rowVersion = _etagService.GetIfMatch();

            memberRoleUpdate.MembershipUserId = userId;

            await _adminUserService.UpdateMemberRoleAsync(adminUserId, memberRoleUpdate, rowVersion, cancellationToken);

            return Ok();
        }
    }
}