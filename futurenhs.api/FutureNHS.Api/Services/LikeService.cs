using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FutureNHS.Api.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeCommand _likeCommand;
        private readonly ILikeDataProvider _likeDataProvider;
        private readonly ILogger<LikeService> _logger;
        private readonly ISystemClock _systemClock;


        public LikeService(ILogger<LikeService> logger, ISystemClock systemClock, ILikeCommand likeCommand, ILikeDataProvider likeDataProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _likeCommand = likeCommand;
            _likeDataProvider = likeDataProvider;
        }

        public async Task LikeEntityAsync(Guid membershipUserId, Guid id, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (Guid.Empty == id) throw new ArgumentOutOfRangeException(nameof(id));

            var now = _systemClock.UtcNow.UtcDateTime;

            var entityLikeDto = new EntityLikeDto()
            {
                Id = id,
                CreatedAtUTC = now,
                MembershipUserId = membershipUserId,
            };

            await _likeCommand.CreateLikedEntityAsync(entityLikeDto, cancellationToken);
        }

        public async Task UnlikeEntityAsync(Guid membershipUserId, Guid id, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (Guid.Empty == id) throw new ArgumentOutOfRangeException(nameof(id));

            var entityLikeDto = new EntityLikeDto()
            {
                Id = id,
                MembershipUserId = membershipUserId,
            };

            await _likeCommand.DeleteLikedEntityAsync(entityLikeDto, cancellationToken);
        }
    }
}
