using FutureNHS.Api.Attributes;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Models.Group.Request;
using FutureNHS.Api.Models.Member.Request;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class GroupsController : ControllerIdentityBase
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IGroupMembershipService _groupMembershipService;
        private readonly IGroupService _groupService;
        private readonly IEtagService _etagService;

        public GroupsController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService, ILogger<GroupsController> logger, IPermissionsService permissionsService,
            IGroupMembershipService groupMembershipService, IGroupService groupService, IEtagService etagService) : base(baseLogger, userService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _groupMembershipService = groupMembershipService;
            _groupService = groupService;
            _etagService = etagService;
        }

        [HttpGet]
        [Route("groups")]

        public async Task<IActionResult> GetGroupsForUserAsync([FromQuery] PaginationFilter filter, [FromQuery] bool isMember = true, CancellationToken cancellationToken = default)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);

            var route = Request.Path.Value;
            route = QueryHelpers.AddQueryString(route, "isMember", isMember.ToString());

            uint total;
            IEnumerable<GroupSummary> groups;

            
            var (totalGroups, groupSummaries) = await _groupService.GetGroupsForUserAsync(identity.MembershipUserId, isMember, filter.Offset, filter.Limit, cancellationToken);

            total = totalGroups;
            groups = groupSummaries;

            var pagedResponse = PaginationHelper.CreatePagedResponse(groups, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("groups/invites")]

        public async Task<IActionResult> GetGroupInvitesForUserAsync([FromQuery] PaginationFilter filter, CancellationToken cancellationToken = default)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var route = Request.Path.Value;

            uint total;
            IEnumerable<GroupInviteSummary> groups;

            
            var (totalGroups, groupSummaries) = await _groupService.GroupInvitesForUserAsync(identity.MembershipUserId, filter.Offset, filter.Limit, cancellationToken);

            total = totalGroups;
            groups = groupSummaries;

            var pagedResponse = PaginationHelper.CreatePagedResponse(groups, filter, total, route);

            return Ok(pagedResponse);
        }
    

        [HttpPut]
        [Route("groups/invite/{inviteId}")]
        public async Task<IActionResult> DeleteGroupInviteForUserAsync(Guid inviteId, CancellationToken cancellationToken = default)
        { 
            var identity = await GetUserIdentityAsync(cancellationToken);
            var rowVersion = _etagService.GetIfMatch();

            await _groupService.DeleteGroupInviteAsync(inviteId, identity.MembershipUserId, rowVersion, cancellationToken);

            return Ok();
        }
        
        [HttpPut]
        [Route("users/{userId}/groups/invite/{inviteId}")]
        public async Task<IActionResult> DeleteGroupInviteAsync(Guid userId, Guid inviteId, CancellationToken cancellationToken = default)
        { 
            var rowVersion = _etagService.GetIfMatch();

            await _groupService.DeleteGroupInviteAsync(inviteId, userId, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("admin/groups")]

        public async Task<IActionResult> AdminGetGroupsAsync([FromQuery] PaginationFilter filter, CancellationToken cancellationToken = default)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var route = Request.Path.Value;

            var (totalGroups, groupSummaries) = await _groupService.AdminGetGroupsAsync(identity.MembershipUserId, filter.Offset, filter.Limit, cancellationToken);

            var total = totalGroups;
            var groups = groupSummaries;

            var pagedResponse = PaginationHelper.CreatePagedResponse(groups, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("groups/{slug}")]
        public async Task<IActionResult> GetGroupAsync(string slug, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var group = await _groupService.GetGroupAsync(slug, identity.MembershipUserId, cancellationToken);

            if (group is null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        [HttpGet]
        [Route("groups/{slug}/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetUpdateGroupAsync(string slug, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var group = await _groupService.GetGroupAsync(identity.MembershipUserId, slug, cancellationToken);

            if (group is null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        [HttpPut]
        [DisableFormValueModelBinding]
        [Route("groups/{slug}/update")]
        public async Task<IActionResult> UpdateGroupAsync(string slug, CancellationToken cancellationToken)
        {
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }
            var identity = await GetUserIdentityAsync(cancellationToken);
            var rowVersion = _etagService.GetIfMatch();
            await _groupService.UpdateGroupMultipartDocument(identity.MembershipUserId, slug, rowVersion, Request.Body, Request.ContentType, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("groups/{slug}/actions")]
        public async Task<IActionResult> GetActionsUserCanPerformInGroupAsync(string slug, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var group = await _groupService.GetGroupAsync(slug, identity.MembershipUserId, cancellationToken);

            if (group is null)
            {
                return NotFound();
            }

            var permissions = await _permissionsService.GetUserPermissionsForGroupAsync(identity.MembershipUserId, group.Id, group.MemberStatus, cancellationToken);

            if (permissions is null)
            {
                return NotFound();
            }

            return Ok(permissions);
        }

        [HttpGet]
        [Route("groups/{slug}/members")]
        public async Task<IActionResult> GetMembersInGroupAsync(string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var route = Request.Path.Value;

            var (total, groupMembers) = await _groupService.GetGroupMembersAsync(identity.MembershipUserId, slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(groupMembers, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("groups/{slug}/members/pending")]
        public async Task<IActionResult> GetPendingMembersInGroupAsync(string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var route = Request.Path.Value;

            var pendingGroupMembers = await _groupService.GetPendingGroupMembersAsync(identity.MembershipUserId, slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            // var (total, pendingGroupMembers) = await _groupService.GetPendingGroupMembersAsync(identity.MembershipUserId, slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);
            //
            // var pagedResponse = PaginationHelper.CreatePagedResponse(pendingGroupMembers, filter, total, route);
            return Ok(pendingGroupMembers);
        }

        [HttpPost]
        [Route("groups/{slug}/members/accept")]
        public async Task<IActionResult> AcceptPendingGroupMemberAsync(string slug, [FromBody] MemberRequest memberRequest, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            await _groupMembershipService.ApproveGroupUserAsync(identity.MembershipUserId, slug, memberRequest.MembershipUserId, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("groups/{slug}/members/reject")]
        public async Task<IActionResult> RejectPendingGroupMemberAsync(string slug, [FromBody] MemberRequest memberRequest, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            await _groupMembershipService.RejectGroupUserAsync(identity.MembershipUserId, slug, memberRequest.MembershipUserId, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("groups/{slug}/members/{targetUserId:guid}")]
        public async Task<IActionResult> GetMemberInGroupAsync(string slug, Guid targetUserId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var member = await _groupService.GetGroupMemberAsync(identity.MembershipUserId, slug, targetUserId, cancellationToken);

            if (member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpGet]
        [Route("groups/{slug}/members/{targetUserId:guid}/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetMemberInGroupForUpdateAsync(string slug, Guid targetUserId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var groupMember = await _groupMembershipService.GetGroupMembershipUserAsync(identity.MembershipUserId, targetUserId, slug, cancellationToken);

            if (groupMember is null)
                return NotFound();

            return Ok(groupMember);
        }

        [HttpPut]
        [Route("groups/{slug}/members/{targetUserId:guid}/roles/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> UpdateMemberInGroupAsync(string slug, Guid targetUserId, [FromBody] UpdateGroupUserRoleRequest updateGroupUserRoleRequest, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var rowVersion = _etagService.GetIfMatch();

            await _groupMembershipService.UpdateGroupMembershipUserRoleAsync(identity.MembershipUserId, slug, targetUserId, updateGroupUserRoleRequest.GroupUserRoleId, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("groups/{slug}/roles")]
        public async Task<IActionResult> GetMembershipRolesForGroupAsync(string slug, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var groupMember = await _groupMembershipService.GetMembershipRolesForGroupAsync(identity.MembershipUserId, slug, cancellationToken);

            if (groupMember is null)
                return NotFound();

            return Ok(groupMember);
        }

        [HttpDelete]
        [Route("groups/{slug}/members/{groupUserId:guid}/delete")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> DeleteMembershipUserFromGroup(string slug, Guid groupUserId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var rowVersion = _etagService.GetIfMatch();

            await _groupMembershipService.DeleteGroupMembershipUserAsync(identity.MembershipUserId, slug, groupUserId, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("groups/{slug}/members/join")]
        public async Task<IActionResult> UserJoinGroupAsync(string slug, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            await _groupMembershipService.UserJoinGroupAsync(identity.MembershipUserId, slug, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("groups/{slug}/members/leave")]
        public async Task<IActionResult> UserLeaveGroupAsync(string slug, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            await _groupMembershipService.UserLeaveGroupAsync(identity.MembershipUserId, slug, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("groups/{slug}/site")]
        public async Task<IActionResult> GetGroupSiteAsync(string slug, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var groupSiteData = await _groupService.GetGroupSiteDataAsync(identity.MembershipUserId, slug, cancellationToken);

            if (groupSiteData is null)
            {
                var createdGroupSiteData = await _groupService.CreateGroupSiteDataAsync(identity.MembershipUserId, slug, cancellationToken);
    
                return Ok(createdGroupSiteData);
            }

            return Ok(groupSiteData);
        }
    }
}
