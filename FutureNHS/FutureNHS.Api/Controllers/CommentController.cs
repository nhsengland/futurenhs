using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentsDataProvider _commentsDataProvider;
        private readonly IPermissionsService _permissionsService;
        private readonly ICommentsCommand _commentsCommand;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public CommentController(ILogger<CommentController> logger, ICommentsDataProvider commentsDataProvider, IPermissionsService permissionsService,
            ICommentsCommand commentsCommand)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _commentsCommand = commentsCommand ?? throw new ArgumentNullException(nameof(commentsCommand));
            _htmlSanitizer = htmlSanitizer;
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{id:guid}/comments")]
        [Route("users/{userId}/groups/{slug}/discussions/{id:guid}/comments")]

        public async Task<IActionResult> GetCommentsForDiscussionAsync(Guid? userId, string slug, Guid id, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var (total, comments) = await _commentsDataProvider.GetCommentsForDiscussionAsync(userId, slug, id, filter.Offset, filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(comments, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments/{id:guid}/replies")]
        [Route("users/{userId}/groups/{slug}/discussions/{discussionId:guid}/comments/{id:guid}/replies")]

        public async Task<IActionResult> GetDiscussionAsync(Guid? userId, string slug, Guid id, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var (total, replies) = await _commentsDataProvider.GetRepliesForCommentAsync(userId, slug, id, filter.Offset, filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(replies, filter, total, route);

           return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("users/{membershipUserId:guid}/groups/{slug}/discussions/{discussionId:guid}/comments")]
        [Route("users/{membershipUserId:guid}/groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}/replies")]
        public async Task<IActionResult> CreateCommentAsync(Guid membershipUserId, string slug, Guid discussionId, Guid? commentId, [FromBody] string comment, CancellationToken cancellationToken)
        {
            var commentDto = new PostDto
            {
                PostContent = comment,
                InReplyTo = commentId,
                TopicId = discussionId,
                MembershipUserId = membershipUserId,
            };

            await _commentsCommand.CreateCommentAsync(commentDto, cancellationToken);

            return Ok();
        }

    }
}