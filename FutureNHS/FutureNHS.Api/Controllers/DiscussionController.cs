using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.Models.Discussion;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class DiscussionController : ControllerBase
    {
        private readonly ILogger<DiscussionController> _logger;
        private readonly IDiscussionDataProvider _discussionDataProvider;
        //private readonly IDiscussionCommand
        private readonly IPermissionsService _permissionsService;

        public DiscussionController(ILogger<DiscussionController> logger, IDiscussionDataProvider discussionDataProvider, IPermissionsService permissionsService)
        {
            _logger = logger;
            _discussionDataProvider = discussionDataProvider;
            _permissionsService = permissionsService;
        }

        [HttpGet]
        [Route("groups/{slug}/discussions")]
        [Route("users/{userId}/groups/{slug}/discussions")]

        public async Task<IActionResult> GetDiscussionsForGroupAsync(Guid? userId, string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var (total, discussions) = await _discussionDataProvider.GetDiscussionsForGroupAsync(userId, slug, filter.Offset, filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(discussions, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{id:guid}")]
        [Route("users/{userId}/groups/{slug}/discussions/{id:guid}")]

        public async Task<IActionResult> GetDiscussionAsync(Guid? userId, string slug, Guid id, CancellationToken cancellationToken)
        {
           var discussion = await _discussionDataProvider.GetDiscussionAsync(userId, slug, id, cancellationToken);

            if (discussion is null)
            {
                return NotFound();
            }

            return Ok(discussion);
        }

        [HttpPost]
        [Route("users/{userId}/groups/{slug}/discussions")]

        public async Task<IActionResult> CreateDiscussionAsync(Guid? userId, string slug, Discussion discussion, CancellationToken cancellationToken)
        {
            //var discussion = await _discussionDataProvider.GetDiscussionAsync(userId, slug, id, cancellationToken);

            if (discussion is null)
            {
                return NotFound();
            }

            return Ok(discussion);
        }
    }
}