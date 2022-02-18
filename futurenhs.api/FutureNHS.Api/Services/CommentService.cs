using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Models.Comment;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services
{
    public class CommentService : ICommentService
    {
        private const string DefaultRole = "Standard Members";
        private const string AddDiscussionRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/discussions/comments/add";

        private readonly ILogger<DiscussionService> _logger;
        private readonly ICommentCommand _commentCommand;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;

        public CommentService(ISystemClock systemClock, ILogger<DiscussionService> logger, IPermissionsService permissionsService, ICommentCommand commentCommand)
        {
            _systemClock = systemClock;
            _commentCommand = commentCommand;
            _permissionsService = permissionsService;
            _logger = logger;
        }

        public async Task CreateCommentAsync(Guid userId, string slug, Guid discussionId, Comment comment, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;
            
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddDiscussionRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateCommentAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var discussionDto = new CommentDto()
            {
                Content = comment.Content,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                FlaggedAsSpam = false,
                InReplyTo = null,
                DiscussionId = discussionId,
                ThreadId = null,
                IsDeleted = false
            };

            await _commentCommand.CreateCommentAsync(discussionDto, cancellationToken);
        }

        public async Task CreateCommentReplyAsync(Guid userId, string slug, Guid discussionId,Guid replyingToComment, Comment comment, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddDiscussionRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateCommentAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var threadId = await _commentCommand.GetThreadIdForComment(replyingToComment, cancellationToken);

            if (!threadId.HasValue)
            {
                _logger.LogError($"Error: CreateCommentReplyAsync - Cannot find the original comment when replying to comment:{0} in discussion: {1}", replyingToComment, discussionId);
                throw new SecurityException($"Error: User does not have access");
            }

            var discussionDto = new CommentDto()
            {
                Content = comment.Content,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                FlaggedAsSpam = false,
                InReplyTo = replyingToComment,
                DiscussionId = discussionId,
                ThreadId = threadId,
                IsDeleted = false
            };

            await _commentCommand.CreateCommentAsync(discussionDto, cancellationToken);
        }
    }
}
