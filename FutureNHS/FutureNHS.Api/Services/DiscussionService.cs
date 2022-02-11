using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FutureNHS.Api.Services
{
    public class GroupMembershipService : IGroupMembershipService
    {
        private const string DefaultRole = "Standard Members";
        private readonly ISystemClock _systemClock;
        private readonly IGroupCommand _groupCommand;
        private readonly IRolesCommand _rolesCommand;

        public GroupMembershipService(ISystemClock systemClock, IGroupCommand groupCommand, IRolesCommand rolesCommand)
        {
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

            var groupUser = new GroupUserDto
            {
                Group = groupId,
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

            await _groupCommand.UserLeaveGroupAsync(userId, groupId, cancellationToken);
        }


    }
}
