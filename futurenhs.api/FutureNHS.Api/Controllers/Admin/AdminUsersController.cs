using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Attributes;
using FutureNHS.Api.Models.Member;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class AdminUsersController : ControllerIdentityBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IAdminUserService _adminUserService;
        private readonly IUserService _userService;
        private readonly IEtagService _etagService;

        public AdminUsersController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService, ILogger<UsersController> logger, 
            IPermissionsService permissionsService,
            IAdminUserService adminUserService,
            IEtagService etagService) : base(baseLogger, userService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _adminUserService = adminUserService;
            _userService = userService;
            _etagService = etagService;
        }

        [HttpGet]
        [Route("admin/users/search")]
        public async Task<IActionResult> SearchMembersAsync(Guid adminUserId, [FromQuery, MinLength(SearchSettings.TermMinimum), MaxLength(SearchSettings.TermMaximum)] string term, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var identity = await GetUserIdentityAsync(cancellationToken);

            term = term.Trim();
            var (total, members) = await _adminUserService.SearchMembersAsync(identity.MembershipUserId, term, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(members, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("admin/users")]
        public async Task<IActionResult> GetMembersAsync(Guid adminUserId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var identity = await GetUserIdentityAsync(cancellationToken);

            var (total, members) = await _adminUserService.GetMembersAsync(identity.MembershipUserId, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(members, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("admin/users/roles")]
        public async Task<IActionResult> GetMembershipRolesAsync(Guid adminUserId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var roles = await _adminUserService.GetMemberRolesAsync(identity.MembershipUserId, cancellationToken);

            if (roles is null)
                return NotFound();

            return Ok(roles);
        }

        [HttpGet]
        [Route("admin/users/{userId:guid}/roles")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetMemberForRoleUpdateAsync(Guid adminUserId, Guid userId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var member = await _adminUserService.GetMemberRoleAsync(identity.MembershipUserId, userId, cancellationToken);

            if (member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpPut]
        [DisableFormValueModelBinding]
        [Route("admin/users/{userId:guid}/roles")]
        public async Task<IActionResult> UpdateMemberRoleAsync(Guid adminUserId, Guid userId, [FromBody] MemberRoleUpdate memberRoleUpdate, CancellationToken cancellationToken)
        {
            var rowVersion = _etagService.GetIfMatch();
            var identity = await GetUserIdentityAsync(cancellationToken);

            memberRoleUpdate.MembershipUserId = userId;

            await _adminUserService.UpdateMemberRoleAsync(identity.MembershipUserId, memberRoleUpdate, rowVersion, cancellationToken);

            return Ok();
        }
        
    }
}