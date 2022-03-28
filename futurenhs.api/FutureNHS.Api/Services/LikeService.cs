using System.Security;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FutureNHS.Api.Services
{
    public class LikeService : ILikeService
    {
        private const string LikeRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/like";

        private readonly ILikeCommand _likeCommand;
        private readonly ILogger<LikeService> _logger;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;


        public LikeService(ILogger<LikeService> logger, ISystemClock systemClock, ILikeCommand likeCommand, IPermissionsService permissionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _likeCommand = likeCommand;
            _permissionsService = permissionsService;
        }

        public async Task LikeEntityAsync(Guid membershipUserId, string slug, Guid id, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (Guid.Empty == id) throw new ArgumentOutOfRangeException(nameof(id));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(membershipUserId, slug, LikeRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: LikeEntityAsync - User:{0} does not have access to edit/delete group:{1}", membershipUserId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var entityLikeDto = new EntityLikeDto()
            {
                Id = id,
                CreatedAtUTC = now,
                MembershipUserId = membershipUserId,
            };

            await _likeCommand.CreateLikedEntityAsync(entityLikeDto, cancellationToken);
        }

        public async Task UnlikeEntityAsync(Guid membershipUserId, string slug, Guid id, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (Guid.Empty == id) throw new ArgumentOutOfRangeException(nameof(id));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(membershipUserId, slug, LikeRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: UnlikeEntityAsync - User:{0} does not have access to edit/delete group:{1}", membershipUserId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var entityLikeDto = new EntityLikeDto()
            {
                Id = id,
                MembershipUserId = membershipUserId,
            };

            await _likeCommand.DeleteLikedEntityAsync(entityLikeDto, cancellationToken);
        }
    }
}
