using FutureNHS.Api.Attributes;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
        private readonly ICommentCommand _commentCommand;
        private readonly ICommentService _commentService;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public CommentController(ILogger<CommentController> logger, ICommentsDataProvider commentsDataProvider, 
            ICommentService commentService, IHtmlSanitizer htmlSanitizer, ICommentCommand commentCommand)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer));
            _commentCommand = commentCommand ?? throw new ArgumentNullException(nameof(commentCommand));
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{discussionsId:guid}/comments/{commentId:guid}")]
        [Route("users/{userId}/groups/{slug}/discussions/{discussionsId:guid}/comments/{commentId:guid}")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetCommentsForDiscussionAsync(Guid? userId, string slug, Guid discussionsId, Guid commentId, CancellationToken cancellationToken)
        {
            var comment = await _commentCommand.GetCommentAsync(commentId, cancellationToken);

            return Ok(comment);
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

        [HttpPut]
        [Route("users/{membershipUserId:guid}/groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}")]
        public async Task<IActionResult> UpdateCommentAsync(Guid membershipUserId, string slug, Guid discussionId, Guid commentId, Comment comment, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var request = Request;            
            if (!request.Headers.ContainsKey(HeaderNames.IfMatch))
            {
                return BadRequest(new { error = "If-Match header not set" });
            }

            byte[] rowVersion = Convert.FromBase64String(request.Headers[HeaderNames.IfMatch].ToString());

            comment.Content = _htmlSanitizer.Sanitize(comment.Content);

            await _commentService.UpdateCommentAsync(membershipUserId, slug, discussionId, commentId, comment, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("users/{membershipUserId:guid}/groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}")]
        public async Task<IActionResult> DeleteCommentAsync(Guid membershipUserId, string slug, Guid discussionId, Guid commentId, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var request = Request;
            if (!request.Headers.ContainsKey(HeaderNames.IfMatch))
            {
                return BadRequest(new { error = "If-Match header not set" });
            }

            byte[] rowVersion = Convert.FromBase64String(request.Headers[HeaderNames.IfMatch].ToString());

            await _commentService.DeleteCommentAsync(membershipUserId, slug, discussionId, commentId, rowVersion, cancellationToken);

            return Ok();
        }
    }
}