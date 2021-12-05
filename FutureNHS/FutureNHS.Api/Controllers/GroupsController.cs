using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Models.Pagination.Services;
using FutureNHS.Application.Application;
using FutureNHS.Infrastructure.Repositories.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Infrastructure.Models;
using FutureNHS.Api.DataAccess.Models.GroupPages;
using FutureNHS.Infrastructure.Models.GroupPages;

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
        private readonly IUriService _uriService;

        public GroupsController(ILogger<GroupsController> logger, IGroupDataProvider groupDataProvider, IUriService uriService)
        {
            _logger = logger;
            _groupDataProvider = groupDataProvider;
            _uriService = uriService;
        }

        [HttpGet]
        [Route("user/{id}/groups")]

        public async Task<IActionResult> GetGroupsForUserAsync(Guid id, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (totalGroups, groupSummaries) = await _groupDataProvider.GetGroupsForUserAsync(id, filter.PageNumber, filter.PageSize, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedReponse(groupSummaries, filter, totalGroups, _uriService, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("user/{id}/discover/groups")]
        public async Task<IActionResult> DiscoverNewGroupsForUserAsync(Guid id, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (totalGroups, groupSummaries) = await _groupDataProvider.DiscoverGroupsForUserAsync(id, filter.PageNumber, filter.PageSize, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedReponse(groupSummaries, filter, totalGroups, _uriService, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("user/{id}/groups/{slug}")]
        public async Task<IActionResult> GetGroupAsync(Guid id, string slug, [FromQuery] string page, CancellationToken cancellationToken)
        {
            //var route = Request.Path.Value;

            //var (totalGroups, groupSummaries = await _groupDataProvider.DiscoverGroupsForUserAsync(id, filter.PageNumber, filter.PageSize, cancellationToken);

            var groupHeader = await _groupDataProvider.GetGroupHeaderForUserAsync(id, slug, cancellationToken);
            if (groupHeader.UserStatus == "Approved-member")
                groupHeader.UserGroupActions = new List<UrlLink> { new UrlLink { Name = "Leave Group", Url = "Leave-the-group-url" } };

            if (page.ToLower() == "home")
            {
                var groupHome = await _groupDataProvider.GetGroupHomePage(slug, cancellationToken);

                var groupHomePage = new GroupPage<GroupHomePage> { PageHeader = groupHeader, PageBody = groupHome };
                return Ok(groupHomePage);
            }

            if (page.ToLower() == "forum")
            {
                var groupForum = new GroupForumPage { SubtitleText = "Forum", BodyHtml = "Holding page for Files & Folders" };

                var groupForumPage = new GroupPage<GroupForumPage> { PageHeader = groupHeader, PageBody = groupForum };
                return Ok(groupForumPage);
            }

            if (page.ToLower() == "files")
            {
                var groupFiles = new GroupFilesPage { SubtitleText = "Files & Folders", BodyHtml = "Holding page for Files & Folders" };

                var groupFilesPage = new GroupPage<GroupFilesPage> { PageHeader = groupHeader, PageBody = groupFiles };
                return Ok(groupFilesPage);
            }

            if (page.ToLower() == "members")
            {
                var groupMembers = new GroupMembersPage { SubtitleText = "Members", BodyHtml = "Holding page for Members" };

                var groupMembersPage = new GroupPage<GroupMembersPage> { PageHeader = groupHeader, PageBody = groupMembers };
                return Ok(groupMembersPage);
            }

            var groupPage = new GroupPage<string> { PageHeader = groupHeader, PageBody = page };
            return Ok(groupPage);
        }
    }
}