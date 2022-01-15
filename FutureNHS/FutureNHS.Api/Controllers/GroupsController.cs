using FutureNHS.Api.DataAccess.Models.GroupUser;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> GetGroupsForUserAsync(Guid userId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (totalGroups, groupSummaries) = await _groupDataProvider.GetGroupsForUserAsync(userId, filter.Offset, filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(groupSummaries, filter, totalGroups,route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("users/{userId:guid}/discover/groups")]
        public async Task<IActionResult> DiscoverNewGroupsForUserAsync(Guid userId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (totalGroups, groupSummaries) = await _groupDataProvider.DiscoverGroupsForUserAsync(userId, filter.Offset, filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(groupSummaries, filter, totalGroups, route);

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
    }
}