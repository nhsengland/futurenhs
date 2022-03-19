using FutureNHS.Api.Attributes;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Helpers;
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
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IPermissionsService _permissionsService;
        private readonly IGroupMembershipService _groupMembershipService;
        private readonly IGroupService _groupService;
        private readonly IEtagService _etagService;

        public GroupsController(ILogger<GroupsController> logger, IGroupDataProvider groupDataProvider,IPermissionsService permissionsService, IGroupMembershipService groupMembershipService, IGroupService groupService, IEtagService etagService)
        {
            _logger = logger;
            _groupDataProvider = groupDataProvider;
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
                var (totalGroups, groupSummaries) = await _groupDataProvider.GetGroupsForUserAsync(userId, filter.Offset, filter.Limit, cancellationToken);
                
                total = totalGroups;
                groups = groupSummaries;

            }
            else
            {
                var (totalGroups, groupSummaries) = await _groupDataProvider.DiscoverGroupsForUserAsync(userId, filter.Offset, filter.Limit, cancellationToken);
                
                total = totalGroups;
                groups = groupSummaries;
            }

            var pagedResponse = PaginationHelper.CreatePagedResponse(groups, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}")]
        public async Task<IActionResult> GetGroupAsync(string slug,Guid userId, CancellationToken cancellationToken)
        {
            var group = await _groupDataProvider.GetGroupAsync(slug, userId, cancellationToken);

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
            await _groupService.UpdateGroupMultipartDocument(userId, slug, rowVersion, Request.Body, Request.ContentType,cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/actions")]
        public async Task<IActionResult> GetActionsUserCanPerformInGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            var group = await _groupDataProvider.GetGroupAsync(slug, userId, cancellationToken);

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

            var (total, groupMembers) = await _groupDataProvider.GetGroupMembersAsync(slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(groupMembers, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/members/pending")]
        public async Task<IActionResult> GetPendingMembersInGroupAsync(Guid userId, string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (total, pendingGroupMembers) = await _groupDataProvider.GetPendingGroupMembersAsync(slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(pendingGroupMembers, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/members/{id:guid}")]
        public async Task<IActionResult> GetMemberInGroupAsync(Guid userId, string slug, Guid Id, CancellationToken cancellationToken)
        {
            var member = await _groupDataProvider.GetGroupMemberAsync(slug, Id, cancellationToken);

            if(member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/members/join")]
        public async Task<IActionResult> UserJoinGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            await _groupMembershipService.UserJoinGroupAsync(userId,slug, cancellationToken);
            
            return Ok();
        }

        [HttpDelete]
        [Route("users/{userId:guid}/groups/{slug}/members/leave")]
        public async Task<IActionResult> UserLeaveGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            await _groupMembershipService.UserLeaveGroupAsync(userId, slug, cancellationToken);

            return Ok();
        }
    }
}