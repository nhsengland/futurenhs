using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Notifications.Interfaces;
using FutureNHS.Api.Services.Validation;
using Microsoft.AspNetCore.Authentication;

namespace FutureNHS.Api.Services
{
    public class GroupMembershipService : IGroupMembershipService
    {
        private const string DefaultRole = "Standard Members";

        private const string EditUserRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/members/edit";
        private const string AddUserRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/members/add";

        private readonly ILogger<GroupMembershipService> _logger;
        private readonly ISystemClock _systemClock;
        private readonly IGroupCommand _groupCommand;
        private readonly IRolesCommand _rolesCommand;
        private readonly IUserCommand _userCommand;
        private readonly IPermissionsService _permissionsService;
        private readonly IGroupMemberNotificationService _groupMemberNotificationService;

        public GroupMembershipService(ILogger<GroupMembershipService> logger,
            ISystemClock systemClock,
            IGroupCommand groupCommand,
            IRolesCommand rolesCommand,
            IUserCommand userCommand,
            IPermissionsService permissionsService,
            IGroupMemberNotificationService groupMemberNotificationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _groupCommand = groupCommand ?? throw new ArgumentNullException(nameof(groupCommand));
            _rolesCommand = rolesCommand ?? throw new ArgumentNullException(nameof(rolesCommand));
            _userCommand = userCommand ?? throw new ArgumentNullException(nameof(userCommand));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _groupMemberNotificationService = groupMemberNotificationService ?? throw new ArgumentNullException(nameof(groupMemberNotificationService));
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
            var group = await _groupCommand.GetGroupAsync(slug, cancellationToken);

            if (group is null)
            {
                _logger.LogError($"Error: UserJoinGroupAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var groupUser = await _groupCommand.GetGroupUserAsync(userId, group.Id, cancellationToken);

            if (groupUser is not null)
            {
                throw new ValidationException(nameof(groupUser.MembershipUser), "User has already requested access to this group");
            }

            groupUser = new GroupUserDto
            {
                Group = group.Id,
                MembershipUser = userId,
                RequestToJoinDateUTC = now,
                Approved = group.IsPublic,
                ApprovingMembershipUser = group.IsPublic ? userId : null,
                ApprovedDateUTC = group.IsPublic ? now : null,
                MembershipRole = defaultRole.Id,
            };

            await _groupCommand.UserJoinGroupAsync(groupUser, cancellationToken);

            if (!group.IsPublic)
            {
                _ = Task.Run(() => _groupMemberNotificationService.SendApplicationNotificationToGroupAdminAsync(slug, cancellationToken), cancellationToken);
            }
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

        public async Task ApproveGroupUserAsync(Guid userId, string slug, Guid targetUserId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));
            if (Guid.Empty == targetUserId) throw new ArgumentOutOfRangeException(nameof(targetUserId));

            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddUserRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: ApproveGroupUserAsync - User:{0} does not have permission to add a new user to this group:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }            

            var group = await _groupCommand.GetGroupAsync(slug, cancellationToken);
            if (group is null)
            {
                _logger.LogError($"Error: ApproveGroupUserAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var groupUser = await _groupCommand.GetGroupUserAsync(targetUserId, group.Id, cancellationToken);

            var groupUserApplicationValidator = new GroupUserApplicationValidator();
            var validationResult = await groupUserApplicationValidator.ValidateAsync(groupUser, cancellationToken);
            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            groupUser.ApprovedDateUTC = now;
            groupUser.ApprovingMembershipUser = userId;

            await _groupCommand.ApproveGroupUserAsync(groupUser, cancellationToken);

            _ = Task.Run(() => _groupMemberNotificationService.SendAcceptNotificationToMemberAsync(targetUserId, group.Name, cancellationToken), cancellationToken);
        }

        public async Task RejectGroupUserAsync(Guid userId, string slug, Guid targetUserId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));
            if (Guid.Empty == targetUserId) throw new ArgumentOutOfRangeException(nameof(targetUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddUserRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: RejectGroupUserAsync - User:{0} does not have permission to add a new user to this group:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            var group = await _groupCommand.GetGroupAsync(slug, cancellationToken);
            if (group is null)
            {
                _logger.LogError($"Error: RejectGroupUserAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var groupUser = await _groupCommand.GetGroupUserAsync(targetUserId, group.Id, cancellationToken);

            var groupUserApplicationValidator = new GroupUserApplicationValidator();
            var validationResult = await groupUserApplicationValidator.ValidateAsync(groupUser, cancellationToken);
            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            await _groupCommand.RejectGroupUserAsync(groupUser.Id, groupUser.RowVersion, cancellationToken);

            _ = Task.Run(() => _groupMemberNotificationService.SendRejectNotificationToMemberAsync(targetUserId, group.Name, cancellationToken), cancellationToken);
        }
    }
}