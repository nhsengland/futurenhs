using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FutureNHS.Api.Services
{
    public class GroupMembershipService : IGroupMembershipService
    {
        private const string DefaultRole = "Standard Members";

        private readonly ILogger<GroupMembershipService> _logger;
        private readonly ISystemClock _systemClock;
        private readonly IGroupCommand _groupCommand;
        private readonly IRolesCommand _rolesCommand;

        public GroupMembershipService(ILogger<GroupMembershipService> logger, ISystemClock systemClock, IGroupCommand groupCommand, IRolesCommand rolesCommand)
        {
            _logger = logger;
            _systemClock = systemClock;
            _groupCommand = groupCommand;
            _rolesCommand = rolesCommand;
        }

        public async Task UserJoinGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var defaultRole = await _rolesCommand.GetRoleAsync(DefaultRole, cancellationToken);
            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: UserJoinGroupAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var groupUser = new GroupUserDto
            {
                Group = groupId.Value,
                MembershipUser = userId,
                RequestToJoinDateUTC = now,
                Approved = true,
                ApprovingMembershipUser = userId,
                ApprovedDateUTC = now,
                MembershipRole = defaultRole.Id,
                
            };

            await _groupCommand.UserJoinGroupAsync(groupUser, cancellationToken);
        }

        public async Task UserLeaveGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: UserLeaveGroupAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            await _groupCommand.UserLeaveGroupAsync(userId, groupId.Value, cancellationToken);
        }


    }
}
