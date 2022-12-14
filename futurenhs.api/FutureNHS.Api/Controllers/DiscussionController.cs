using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Models.Discussion;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class DiscussionController : ControllerIdentityBase
    {
        private readonly ILogger<DiscussionController> _logger;
        private readonly IDiscussionDataProvider _discussionDataProvider;
        private readonly IDiscussionService _discussionService;
        private readonly IPermissionsService _permissionsService;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public DiscussionController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService, ILogger<DiscussionController> logger, IDiscussionDataProvider discussionDataProvider,
            IPermissionsService permissionsService, IDiscussionService discussionService, IHtmlSanitizer htmlSanitizer) : base(baseLogger, userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _discussionDataProvider = discussionDataProvider ?? throw new ArgumentNullException(nameof(discussionDataProvider)); ;
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService)); ;
            _discussionService = discussionService ?? throw new ArgumentNullException(nameof(discussionService)); ;
            _htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer)); ;
        }

        [HttpGet]
        [Route("groups/{slug}/discussions")]

        public async Task<IActionResult> GetDiscussionsForGroupAsync(string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var identity = await GetUserIdentityAsync(cancellationToken);
            var discussions = await _discussionService.GetDiscussionsForGroupAsync(identity.MembershipUserId, slug, filter.Offset, filter.Limit, filter.Sort, cancellationToken);
            var total = await _discussionDataProvider.GetDiscussionCountForGroupAsync(slug, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(discussions, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{id:guid}")]

        public async Task<IActionResult> GetDiscussionAsync(Guid? userId, string slug, Guid id, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var discussion = await _discussionDataProvider.GetDiscussionAsync(identity.MembershipUserId, slug, id, cancellationToken);

            if (discussion is null)
            {
                return NotFound();
            }

            return Ok(discussion);
        }

        [HttpPost]
        [Route("groups/{slug}/discussions")]
        public async Task<IActionResult> CreateDiscussionAsync(Guid userId, string slug, Discussion discussion, CancellationToken cancellationToken)
        {
            discussion.Content = _htmlSanitizer.Sanitize(discussion.Content);
            var identity = await GetUserIdentityAsync(cancellationToken);

            await _discussionService.CreateDiscussionAsync(identity.MembershipUserId, slug, discussion, cancellationToken);

            return Ok();
        }
    }
}