using FutureNHS.Api.Attributes;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Comment = FutureNHS.Api.Models.Comment.Comment;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class CommentController : ControllerIdentityBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentsDataProvider _commentsDataProvider;
        private readonly ICommentCommand _commentCommand;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IEtagService _etagService;

        public CommentController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService, ILogger<CommentController> logger, ICommentsDataProvider commentsDataProvider,
            ICommentService commentService, IHtmlSanitizer htmlSanitizer, ICommentCommand commentCommand,
            IEtagService etagService, ILikeService likeService) : base(baseLogger, userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer));
            _commentCommand = commentCommand ?? throw new ArgumentNullException(nameof(commentCommand));
            _etagService = etagService ?? throw new ArgumentNullException(nameof(etagService));
            _likeService = likeService ?? throw new ArgumentNullException(nameof(likeService)); ;
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{discussionsId:guid}/comments/{commentId:guid}")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetCommentsForDiscussionAsync(Guid? userId, string slug, Guid discussionsId, Guid commentId, CancellationToken cancellationToken)
        {
            var comment = await _commentCommand.GetCommentAsync(commentId, cancellationToken);

            return Ok(comment);
        }
        
        [HttpGet]
        [Route("groups/{slug}/discussions/{discussionsId:guid}/comments/{commentId:guid}/likes")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetLikesForCommentAsync(string slug, Guid commentId, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var likes = await _likeService.GetLikesForEntityAsync(identity.MembershipUserId, slug, commentId, cancellationToken);

            return Ok(likes);
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments")]
        public async Task<IActionResult> GetCommentsForDiscussionAsync(string slug, Guid discussionId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var route = Request.Path.Value;

            var comments = await _commentsDataProvider.GetCommentsForDiscussionAsync(identity.MembershipUserId, slug, discussionId, filter.Offset, filter.Limit, cancellationToken);
            var total = await _commentsDataProvider.GetCommentsCountForDiscussionAsync(slug, discussionId, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(comments, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}/replies")]
        public async Task<IActionResult> GetRepliesForCommentAsync(Guid? userId, string slug, Guid commentId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var route = Request.Path.Value;

            var replies = await _commentsDataProvider.GetRepliesForCommentAsync(identity.MembershipUserId, slug, commentId, filter.Offset, filter.Limit, cancellationToken);
            var total = await _commentsDataProvider.GetRepliesCountForCommentAsync(slug, commentId, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(replies, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments")]
        public async Task<IActionResult> CreateCommentAsync(Guid membershipUserId, string slug, Guid discussionId, Comment comment, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            comment.Content = _htmlSanitizer.Sanitize(comment.Content);

            var identity = await GetUserIdentityAsync(cancellationToken);
            var commentId = await _commentService.CreateCommentAsync(identity.MembershipUserId, slug, discussionId, comment, cancellationToken);

            return Ok(commentId);
        }

        [HttpPost]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}/replies")]
        public async Task<IActionResult> CreateCommentReplyAsync(Guid membershipUserId, string slug, Guid discussionId, Guid commentId, Comment comment, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            comment.Content = _htmlSanitizer.Sanitize(comment.Content);

            var identity = await GetUserIdentityAsync(cancellationToken);
            var childCommentId = await _commentService.CreateCommentReplyAsync(identity.MembershipUserId, slug, discussionId, commentId, comment, cancellationToken);

            return Ok(childCommentId);
        }

        [HttpPut]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}")]
        public async Task<IActionResult> UpdateCommentAsync(Guid membershipUserId, string slug, Guid discussionId, Guid commentId, Comment comment, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            comment.Content = _htmlSanitizer.Sanitize(comment.Content);
            var rowVersion = _etagService.GetIfMatch();
            var identity = await GetUserIdentityAsync(cancellationToken);
            await _commentService.UpdateCommentAsync(identity.MembershipUserId, slug, discussionId, commentId, comment, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}")]
        public async Task<IActionResult> DeleteCommentAsync(Guid membershipUserId, string slug, Guid discussionId, Guid commentId, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var identity = await GetUserIdentityAsync(cancellationToken);
            var rowVersion = _etagService.GetIfMatch();
            await _commentService.DeleteCommentAsync(identity.MembershipUserId, slug, discussionId, commentId, rowVersion, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}/like")]
        public async Task<IActionResult> LikeCommentAsync(Guid membershipUserId, string slug, Guid commentId, Guid discussionId, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var identity = await GetUserIdentityAsync(cancellationToken);
            await _likeService.LikeEntityAsync(identity.MembershipUserId, slug, commentId, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Route("groups/{slug}/discussions/{discussionId:guid}/comments/{commentId:guid}/unlike")]
        public async Task<IActionResult> UnlikeCommentAsync(Guid membershipUserId, string slug, Guid commentId, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var identity = await GetUserIdentityAsync(cancellationToken);
            await _likeService.UnlikeEntityAsync(identity.MembershipUserId, slug, commentId, cancellationToken);

            return Ok();
        }
    }
}
