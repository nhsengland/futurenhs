using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Comment = FutureNHS.Api.Models.Comment.Comment;

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
        private readonly ICommentService _commentService;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public CommentController(ILogger<CommentController> logger, ICommentsDataProvider commentsDataProvider, IPermissionsService permissionsService, ICommentService commentService, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
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
        public async Task<IActionResult> CreateCommentAsync(Guid membershipUserId, string slug, Guid discussionId, Comment comment, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            comment.Content = _htmlSanitizer.Sanitize(comment.Content);

            await _commentService.CreateCommentAsync(membershipUserId, slug, discussionId, comment, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("users/{membershipUserId:guid}/groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}/replies")]
        public async Task<IActionResult> CreateCommentReplyAsync(Guid membershipUserId, string slug, Guid discussionId, Guid commentId, Comment comment, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            comment.Content = _htmlSanitizer.Sanitize(comment.Content);

            await _commentService.CreateCommentReplyAsync(membershipUserId, slug, discussionId, commentId, comment, cancellationToken);

            return Ok();
        }

    }
}