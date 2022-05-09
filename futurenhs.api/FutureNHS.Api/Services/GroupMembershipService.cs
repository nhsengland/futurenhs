using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.User;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services
{
    public class GroupMembershipService : IGroupMembershipService
    {
        private const string DefaultRole = "Standard Members";

        private const string EditUserRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/members/edit";

        private readonly ILogger<GroupMembershipService> _logger;
        private readonly ISystemClock _systemClock;
        private readonly IGroupCommand _groupCommand;
        private readonly IRolesCommand _rolesCommand;
        private readonly IUserCommand _userCommand;
        private readonly IPermissionsService _permissionsService;

        public GroupMembershipService(ILogger<GroupMembershipService> logger, ISystemClock systemClock, IGroupCommand groupCommand,
            IRolesCommand rolesCommand, IUserCommand userCommand, IPermissionsService permissionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _groupCommand = groupCommand ?? throw new ArgumentNullException(nameof(groupCommand));
            _rolesCommand = rolesCommand ?? throw new ArgumentNullException(nameof(rolesCommand));
            _userCommand = userCommand ?? throw new ArgumentNullException(nameof(userCommand));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
        }

        public async Task<GroupMemberDetails> GetGroupMembershipUserAsync(Guid userId, Guid targetUserId, string slug, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == targetUserId) throw new ArgumentOutOfRangeException(nameof(targetUserId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, EditUserRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetGroupMembershipUserAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: GetGroupMembershipUserAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var groupUserResult = await _groupCommand.GetGroupUserAsync(targetUserId, groupId.Value, cancellationToken);
            if (groupUserResult is null)
            {
                _logger.LogError($"Error: GetGroupMembershipUserAsync - User:{0} not found in group:{1}", targetUserId, slug);
                throw new KeyNotFoundException("Error: User not found in group");
            }

            var userResult = await _userCommand.GetUserAsync(groupUserResult.MembershipUser, cancellationToken);
            if (userResult is null)
            {
                _logger.LogError($"Error: GetGroupMembershipUserAsync - User:{0} not found in group:{1}", groupUserResult.MembershipUser, slug);
                throw new KeyNotFoundException("Error: User not found in group");
            }

            var roleResult = await _rolesCommand.GetRoleAsync(groupUserResult.MembershipRole, cancellationToken);
            if (roleResult is null)
            {
                _logger.LogError($"Error: GetGroupMembershipUserAsync - Role:{0} not found in group:{1}", groupUserResult.MembershipRole, slug);
                throw new KeyNotFoundException("Error: User not found in group");
            }

            var membershipUser = new GroupMemberDetails
            {
                Id = userResult.Id,
                UserName = userResult.UserName,
                Slug = userResult.Slug,
                FirstName = userResult.FirstName,
                LastName = userResult.Surname,
                Initials = userResult.Initials,
                Email = userResult.Email,                
                DateJoinedUtc = groupUserResult.ApprovedDateUTCAsString,
                LastLoginUtc = userResult.LastLoginDateUtc,
                RoleId = roleResult.Id,
                Role = roleResult.Name,
                RowVersion = groupUserResult.RowVersion
            };

            return membershipUser;
        }

        public async Task UpdateGroupMembershipUserRoleAsync(Guid userId, string slug, Guid targetUserId, Guid groupRoleId, byte[] rowVersion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));
            if (Guid.Empty == targetUserId) throw new ArgumentOutOfRangeException(nameof(targetUserId));
            if (Guid.Empty == groupRoleId) throw new ArgumentOutOfRangeException(nameof(groupRoleId));

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);
            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: UpdateGroupMembershipUserRoleAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var groupUserDto = await _groupCommand.GetGroupUserAsync(targetUserId, groupId.Value, cancellationToken);
            if (!groupUserDto.RowVersion.SequenceEqual(rowVersion))
            {
                _logger.LogError($"Precondition Failed: UpdateGroupMembershipUserRoleAsync - GroupUser:{0} has changed prior to submission ", targetUserId);
                throw new PreconditionFailedExeption("Precondition Failed: GroupUser has changed prior to submission");
            }

            if (userId == groupUserDto.MembershipUser)
            {
                _logger.LogError($"Error: UpdateGroupMembershipUserRoleAsync - User:{0} User cannot edit their own group role", userId);
                throw new ValidationException(nameof(targetUserId), "User cannot edit their own group role");
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, EditUserRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: UpdateGroupMembershipUserRoleAsync - User:{0} does not have access to group:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            await _groupCommand.UpdateUserGroupRolesAsync(groupUserDto.Id, groupRoleId, rowVersion, cancellationToken);
        }

        public async Task<IEnumerable<GroupRoleDto>> GetMembershipRolesForGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, EditUserRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetGroupMembershipUserAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            return await _groupCommand.GetAllGroupRolesAsync(cancellationToken);
        }

        public async Task DeleteGroupMembershipUserAsync(Guid userId, string slug, Guid targetUserId, byte[] rowVersion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));
            if (Guid.Empty == targetUserId) throw new ArgumentOutOfRangeException(nameof(targetUserId));

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);
            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: DeleteGroupMembershipUserAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var groupUserDto = await _groupCommand.GetGroupUserAsync(targetUserId, groupId.Value, cancellationToken);
            if (!groupUserDto.RowVersion.SequenceEqual(rowVersion))
            {
                _logger.LogError($"Precondition Failed: DeleteGroupMembershipUserAsync - GroupUser:{0} has changed prior to submission ", targetUserId);
                throw new PreconditionFailedExeption("Precondition Failed: GroupUser has changed prior to submission");
            }

            if (userId == groupUserDto.MembershipUser)
            {
                _logger.LogError($"Error: DeleteGroupMembershipUserAsync - User:{0} User cannot delete themselves from a group", userId);
                throw new ValidationException(nameof(userId), "User cannot delete themselves from a group");
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, EditUserRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: DeleteGroupMembershipUserAsync - User:{0} does not have access to group:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            await _groupCommand.DeleteUserFromGroupAsync(groupUserDto.Id, rowVersion, cancellationToken);
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
