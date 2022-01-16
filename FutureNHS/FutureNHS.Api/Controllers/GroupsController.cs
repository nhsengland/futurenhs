using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
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

        public GroupsController(ILogger<GroupsController> logger, IGroupDataProvider groupDataProvider,IPermissionsService permissionsService)
        {
            _logger = logger;
            _groupDataProvider = groupDataProvider;
            _permissionsService = permissionsService;
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
        [Route("groups/{slug}")]
        public async Task<IActionResult> GetGroupAsync(string slug, CancellationToken cancellationToken)
        {
            var group = await _groupDataProvider.GetGroupAsync(slug, cancellationToken);

            if (group is null)
            { 
                return NotFound();
            }

            return Ok(group);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/actions")]
        public async Task<IActionResult> GetActionsUserCanPerformInGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            var group = await _groupDataProvider.GetGroupAsync(slug, cancellationToken);

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
    }
}