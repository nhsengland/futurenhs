using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services
{
    public class LikeService : ILikeService
    {
        private const string LikeRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/like";

        private readonly ILikeCommand _likeCommand;
        private readonly ILogger<LikeService> _logger;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;

        public LikeService(ILogger<LikeService> logger, ISystemClock systemClock, ILikeCommand likeCommand, IPermissionsService permissionsService, ILikeDataProvider likeDataProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _likeCommand = likeCommand;
            _permissionsService = permissionsService;
        }

        public async Task LikeEntityAsync(Guid membershipUserId, string slug, Guid commentId, Guid entityId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (Guid.Empty == commentId) throw new ArgumentOutOfRangeException(nameof(commentId));
            if (Guid.Empty == entityId) throw new ArgumentOutOfRangeException(nameof(entityId));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(membershipUserId, slug, LikeRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: LikeEntityAsync - User:{0} does not have access to edit/delete group:{1}", membershipUserId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var entityLikeDto = new EntityLikeDto()
            {
                CommentId = commentId,
                EntityId = entityId,
                CreatedAtUTC = now,
                MembershipUserId = membershipUserId,
            };

            await _likeCommand.CreateLikedEntityAsync(entityLikeDto, cancellationToken);
        }

        public async Task UnlikeEntityAsync(Guid membershipUserId, string slug, Guid commentId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (Guid.Empty == commentId) throw new ArgumentOutOfRangeException(nameof(commentId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(membershipUserId, slug, LikeRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: UnlikeEntityAsync - User:{0} does not have access to edit/delete group:{1}", membershipUserId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var entityLikeDto = new EntityLikeDto()
            {
                CommentId = commentId,
                MembershipUserId = membershipUserId,
            };

            await _likeCommand.DeleteLikedEntityAsync(entityLikeDto, cancellationToken);
        }
    }
}
