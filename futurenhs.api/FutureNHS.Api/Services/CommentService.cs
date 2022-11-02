using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Comment;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Notifications.Interfaces;
using FutureNHS.Api.Services.Validation;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services
{
    public class CommentService : ICommentService
    {
        private const string DefaultRole = "Standard Members";

        private const string AddCommentRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/discussions/comments/add";
        private const string EditCommentRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/discussions/comments/edit";
        private const string DeleteCommentRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/discussions/comments/delete";

        private readonly ILogger<CommentService> _logger;
        private readonly ICommentCommand _commentCommand;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;
        private readonly ICommentNotificationService _commentNotificationService;

        public CommentService(ISystemClock systemClock, 
            ILogger<CommentService> logger,
            IPermissionsService permissionsService,
            ICommentCommand commentCommand,
            ICommentNotificationService  commentNotificationService)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _commentCommand = commentCommand ?? throw new ArgumentNullException(nameof(commentCommand));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentNotificationService = commentNotificationService ?? throw new ArgumentNullException(nameof(commentNotificationService));
        }

        public async Task<Guid> CreateCommentAsync(Guid userId, string slug, Guid parentEntityId, Comment comment, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddCommentRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateCommentAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }
            var entityId = Guid.NewGuid();

            var commentDto = new CommentDto()
            {
                EntityId = entityId,
                Content = comment.Content,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                FlaggedAsSpam = false,
                InReplyTo = null,
                ThreadId = null,
                IsDeleted = false,
                DiscussionId = parentEntityId
            };

            var validator = new CommentValidator();
            var validationResult = await validator.ValidateAsync(commentDto, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);
            
            var createdComment = await _commentCommand.CreateCommentAsync(commentDto, cancellationToken);

            _ = _commentNotificationService.SendNotificationToDiscussionCreatorAsync(userId, parentEntityId, cancellationToken);

            return createdComment;
        }

        public async Task<Guid> CreateCommentReplyAsync(Guid userId, string slug, Guid parentEntityId, Guid replyingToComment, Comment comment, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddCommentRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateCommentAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var threadId = await _commentCommand.GetThreadIdForComment(replyingToComment, cancellationToken);

            if (!threadId.HasValue)
            {
                _logger.LogError($"Error: CreateCommentReplyAsync - Cannot find the original comment when replying to comment:{0} in discussion: {1}", replyingToComment, parentEntityId);
                throw new SecurityException($"Error: User does not have access");
            }
            var entityId = Guid.NewGuid();

            var commentDto = new CommentDto()
            {
                EntityId = entityId,
                Content = comment.Content,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                FlaggedAsSpam = false,
                InReplyTo = replyingToComment,
                ThreadId = threadId,
                IsDeleted = false,
                DiscussionId = parentEntityId
            };

            var validator = new CommentValidator();
            var validationResult = await validator.ValidateAsync(commentDto, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            var createdComment = await _commentCommand.CreateCommentAsync(commentDto, cancellationToken);

            _ = Task.Run(() => _commentNotificationService.SendNotificationToCommentCreatorAsync(userId, replyingToComment, cancellationToken), cancellationToken);

            return createdComment;
        }

        public async Task UpdateCommentAsync(Guid userId, string slug, Guid parentEntityId, Guid commentId, Comment comment, byte[] rowVersion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));
            if (Guid.Empty == parentEntityId) throw new ArgumentOutOfRangeException(nameof(parentEntityId));
            if (Guid.Empty == commentId) throw new ArgumentOutOfRangeException(nameof(commentId));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanAddComment = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddCommentRole, cancellationToken);
            if (!userCanAddComment)
            {
                _logger.LogError($"Forbidden: UpdateCommentAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new ForbiddenException($"Forbidden: User does not have access to this group");
            }

            var userCanEditComment = await _permissionsService.UserCanPerformActionAsync(userId, slug, EditCommentRole, cancellationToken);
            var databaseCommentDto = await _commentCommand.GetCommentAsync(commentId, cancellationToken);
            if (databaseCommentDto.CreatedById != userId && !userCanEditComment)
            {
                _logger.LogError($"Forbidden: UpdateCommentAsync - User:{0} does not have permission to edit comment:{1}", userId, commentId);
                throw new ForbiddenException("Forbidden: User does not have permission to edit this comment");
            }

            if (!databaseCommentDto.RowVersion.SequenceEqual(rowVersion))
            {
                _logger.LogError($"Precondition Failed: UpdateCommentAsync - Comment:{0} has changed prior to submission ", commentId);
                throw new PreconditionFailedExeption("Precondition Failed: Comment has changed prior to submission");
            }

            var commentDto = new CommentDto()
            {
                EntityId = databaseCommentDto.Id,
                Content = comment.Content,
                ModifiedBy = userId,
                ModifiedAtUTC = now,
            };

            var validator = new CommentValidator();
            var validationResult = await validator.ValidateAsync(commentDto, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            await _commentCommand.UpdateCommentAsync(commentDto, rowVersion, cancellationToken);
        }

        public async Task DeleteCommentAsync(Guid userId, string slug, Guid parentEntityId, Guid commentId, byte[] rowVersion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));
            if (Guid.Empty == parentEntityId) throw new ArgumentOutOfRangeException(nameof(parentEntityId));
            if (Guid.Empty == commentId) throw new ArgumentOutOfRangeException(nameof(commentId));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanAddComment = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddCommentRole, cancellationToken);
            if (!userCanAddComment)
            {
                _logger.LogError($"Forbidden: DeleteCommentAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new ForbiddenException($"Forbidden: User does not have access to this group");
            }

            var userCanDeleteComment = await _permissionsService.UserCanPerformActionAsync(userId, slug, DeleteCommentRole, cancellationToken);
            var databaseCommentDto = await _commentCommand.GetCommentAsync(commentId, cancellationToken);
            if (databaseCommentDto.CreatedById != userId && !userCanDeleteComment)
            {
                _logger.LogError($"Forbidden: DeleteCommentAsync - User:{0} does not have permission to delete comment:{1}", userId, commentId);
                throw new ForbiddenException("Forbidden: User does not have permission to delete this comment");
            }

            if (!databaseCommentDto.RowVersion.SequenceEqual(rowVersion))
            {
                _logger.LogError($"Precondition Failed: DeleteCommentAsync - Comment:{0} has changed prior to submission", commentId);
                throw new PreconditionFailedExeption("Precondition Failed: Comment has changed prior to submission");
            }

            var commentDto = new CommentDto()
            {
                EntityId = databaseCommentDto.Id,
                ModifiedBy = userId,
                ModifiedAtUTC = now,
            };

            await _commentCommand.DeleteCommentAsync(commentDto, rowVersion, cancellationToken);
        }
    }
}
