using FutureNHS.Api.Attributes;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Models.Group.Request;
using FutureNHS.Api.Models.Member.Request;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IGroupMembershipService _groupMembershipService;
        private readonly IGroupService _groupService;
        private readonly IEtagService _etagService;

        public GroupsController(ILogger<GroupsController> logger, IPermissionsService permissionsService,
            IGroupMembershipService groupMembershipService, IGroupService groupService, IEtagService etagService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _groupMembershipService = groupMembershipService;
            _groupService = groupService;
            _etagService = etagService;
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups")]

        public async Task<IActionResult> GetGroupsForUserAsync(Guid userId, [FromQuery] PaginationFilter filter, [FromQuery] bool isMember = true, CancellationToken cancellationToken = default)
        {
            var route = Request.Path.Value;
            route = QueryHelpers.AddQueryString(route, "isMember", isMember.ToString());

            uint total;
            IEnumerable<GroupSummary> groups;

            if (isMember)
            {
                var (totalGroups, groupSummaries) = await _groupService.GetGroupsForUserAsync(userId, filter.Offset, filter.Limit, cancellationToken);

                total = totalGroups;
                groups = groupSummaries;

            }
            else
            {
                var (totalGroups, groupSummaries) = await _groupService.DiscoverGroupsForUserAsync(userId, filter.Offset, filter.Limit, cancellationToken);

                total = totalGroups;
                groups = groupSummaries;
            }

            var pagedResponse = PaginationHelper.CreatePagedResponse(groups, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{userId:guid}/admin/groups")]

        public async Task<IActionResult> AdminGetGroupsAsync(Guid userId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken = default)
        {
            var route = Request.Path.Value;

            var (totalGroups, groupSummaries) = await _groupService.AdminGetGroupsAsync(userId, filter.Offset, filter.Limit, cancellationToken);

            var total = totalGroups;
            var groups = groupSummaries;

            var pagedResponse = PaginationHelper.CreatePagedResponse(groups, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}")]
        public async Task<IActionResult> GetGroupAsync(string slug, Guid userId, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupAsync(slug, userId, cancellationToken);

            if (group is null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetUpdateGroupAsync(string slug, Guid userId, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupAsync(userId, slug, cancellationToken);

            if (group is null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        [HttpPut]
        [DisableFormValueModelBinding]
        [Route("users/{userId:guid}/groups/{slug}/update")]
        public async Task<IActionResult> UpdateGroupAsync(string slug, Guid userId, CancellationToken cancellationToken)
        {
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }
            var rowVersion = _etagService.GetIfMatch();
            await _groupService.UpdateGroupMultipartDocument(userId, slug, rowVersion, Request.Body, Request.ContentType, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/actions")]
        public async Task<IActionResult> GetActionsUserCanPerformInGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupAsync(slug, userId, cancellationToken);

            if (group is null)
            {
                return NotFound();
            }

            var permissions = await _permissionsService.GetUserPermissionsForGroupAsync(userId, group.Id, cancellationToken);

            if (permissions is null)
            {
                return NotFound();
            }

            return Ok(permissions);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/members")]
        public async Task<IActionResult> GetMembersInGroupAsync(Guid userId, string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (total, groupMembers) = await _groupService.GetGroupMembersAsync(userId, slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(groupMembers, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/members/pending")]
        public async Task<IActionResult> GetPendingMembersInGroupAsync(Guid userId, string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (total, pendingGroupMembers) = await _groupService.GetPendingGroupMembersAsync(userId, slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(pendingGroupMembers, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/members/accept")]
        public async Task<IActionResult> AcceptPendingGroupMemberAsync(Guid userId, string slug, [FromBody] MemberRequest memberRequest, CancellationToken cancellationToken)
        {
            await _groupMembershipService.ApproveGroupUserAsync(userId, slug, memberRequest.MembershipUserId, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/members/reject")]
        public async Task<IActionResult> RejectPendingGroupMemberAsync(Guid userId, string slug, [FromBody] MemberRequest memberRequest, CancellationToken cancellationToken)
        {
            await _groupMembershipService.RejectGroupUserAsync(userId, slug, memberRequest.MembershipUserId, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/members/{id:guid}")]
        public async Task<IActionResult> GetMemberInGroupAsync(Guid userId, string slug, Guid Id, CancellationToken cancellationToken)
        {
            var member = await _groupService.GetGroupMemberAsync(userId, slug, Id, cancellationToken);

            if (member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/members/{targetUserId:guid}/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetMemberInGroupForUpdateAsync(Guid userId, string slug, Guid targetUserId, CancellationToken cancellationToken)
        {
            var groupMember = await _groupMembershipService.GetGroupMembershipUserAsync(userId, targetUserId, slug, cancellationToken);

            if (groupMember is null)
                return NotFound();

            return Ok(groupMember);
        }

        [HttpPut]
        [Route("users/{userId:guid}/groups/{slug}/members/{targetUserId:guid}/roles/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> UpdateMemberInGroupAsync(Guid userId, string slug, Guid targetUserId, [FromBody] UpdateGroupUserRoleRequest updateGroupUserRoleRequest, CancellationToken cancellationToken)
        {
            var rowVersion = _etagService.GetIfMatch();

            await _groupMembershipService.UpdateGroupMembershipUserRoleAsync(userId, slug, targetUserId, updateGroupUserRoleRequest.GroupUserRoleId, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/roles")]
        public async Task<IActionResult> GetMembershipRolesForGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            var groupMember = await _groupMembershipService.GetMembershipRolesForGroupAsync(userId, slug, cancellationToken);

            if (groupMember is null)
                return NotFound();

            return Ok(groupMember);
        }

        [HttpDelete]
        [Route("users/{userId:guid}/groups/{slug}/members/{groupUserId:guid}/delete")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> DeleteMembershipUserFromGroup(Guid userId, string slug, Guid groupUserId, CancellationToken cancellationToken)
        {
            var rowVersion = _etagService.GetIfMatch();

            await _groupMembershipService.DeleteGroupMembershipUserAsync(userId, slug, groupUserId, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/members/join")]
        public async Task<IActionResult> UserJoinGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            await _groupMembershipService.UserJoinGroupAsync(userId, slug, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("users/{userId:guid}/groups/{slug}/members/leave")]
        public async Task<IActionResult> UserLeaveGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            await _groupMembershipService.UserLeaveGroupAsync(userId, slug, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/site")]
        public async Task<IActionResult> GetGroupSiteAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            var groupSiteData = await _groupService.GetGroupSiteDataAsync(userId, slug, cancellationToken);

            if (groupSiteData is null)
                return NotFound();

            return Ok(groupSiteData);
        }
    }
}
