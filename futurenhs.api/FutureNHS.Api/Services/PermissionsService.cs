﻿using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.Permissions;
using FutureNHS.Api.Services.Interfaces;

namespace FutureNHS.Api.Services
{
    public class PermissionsService : IPermissionsService
    {
        private const string GuestRole = "Guest";
        private const string AdminRole = "Admin";
        private const string Approved = "approved";
        private const string Banned = "banned";
        private const string Locked = "locked";
        private const string Rejected = "rejected";
        private const string PendingApproval = "pending";

        private readonly IPermissionsDataProvider _permissionsDataProvider;
        private readonly IRolesDataProvider _roleDataProvider;
        private readonly ILogger<PermissionsService> _logger;
        private readonly IGroupDataProvider _groupDataProvider;

        public PermissionsService(IRolesDataProvider roleDataProvider, IPermissionsDataProvider permissionsDataProvider, ILogger<PermissionsService> logger, IGroupDataProvider groupDataProvider)
        {
            _permissionsDataProvider = permissionsDataProvider ?? throw new ArgumentNullException(nameof(permissionsDataProvider));
            _roleDataProvider = roleDataProvider ?? throw new ArgumentNullException(nameof(roleDataProvider));
            _logger = logger;
            _groupDataProvider = groupDataProvider ?? throw new ArgumentNullException(nameof(groupDataProvider));
        }

        public async Task<IEnumerable<string>?> GetUserPermissionsForGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Cannot be EMPTY", nameof(userId));
            if (groupId == Guid.Empty) throw new ArgumentException("Cannot be EMPTY", nameof(groupId));

            cancellationToken.ThrowIfCancellationRequested();

            // check if group is public
            bool isPublicGroup = await _groupDataProvider.GetGroupPrivacyStatusAsync(groupId, cancellationToken);

            var (userRoles, groupUserRoles) = await _roleDataProvider.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken);

            if (userRoles is null || !userRoles.Any())
            {
                return null;
            }

            var permissions = new List<string>();

            permissions.AddRange(await GetSiteUserPermissionsForGroupRoles(userRoles));

            if (userRoles.Any(x => x == AdminRole))
                return permissions.Distinct();

            permissions.AddRange(await GetUserPermissionsForGroupRoles(groupUserRoles, groupId, isPublicGroup));

            return permissions.Distinct();
        }

        public async Task<IEnumerable<string>?> GetUserPermissionsForGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Cannot be EMPTY", nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentException("Cannot be EMPTY", nameof(slug));

            cancellationToken.ThrowIfCancellationRequested();

            // check if group is public
            bool isPublicGroup = await _groupDataProvider.GetGroupPrivacyStatusAsync(slug, cancellationToken);

            var (userRoles, groupUserRoles) = await _roleDataProvider.GetUserAndGroupUserRolesAsync(userId, slug, cancellationToken);

            if (userRoles is null || !userRoles.Any())
            {
                return null;
            }

            var permissions = new List<string>();

            permissions.AddRange(await GetSiteUserPermissionsForGroupRoles(userRoles));

            if (userRoles.Any(x => x == AdminRole))
                return permissions.Distinct();

            permissions.AddRange(await GetUserPermissionsForGroupRoles(groupUserRoles, slug, isPublicGroup));

            return permissions.Distinct();
        }

        public async Task<IEnumerable<string>?> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Cannot be EMPTY", nameof(userId));

            cancellationToken.ThrowIfCancellationRequested();

            var userRoles = await _roleDataProvider.GetUserRolesAsync(userId, cancellationToken);

            if (userRoles is null || !userRoles.Any())
            {
                return null;
            }

            var permissions = await GetSitePermissionsForRoles(userRoles.ToList());

            return permissions.Distinct();
        }

        public async Task<bool> UserCanPerformActionAsync(Guid userId, string action, CancellationToken cancellationToken)
        {
            var roles = await GetUserPermissionsAsync(userId, cancellationToken);

            return roles is not null && roles.Any(x => x == action);
        }

        public async Task<bool> UserCanPerformActionAsync(Guid userId, Guid groupId, string action, CancellationToken cancellationToken)
        {
            var roles = await GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

            return roles is not null && roles.Any(x => x == action);
        }

        public async Task<bool> UserCanPerformActionAsync(Guid userId, string slug, string action, CancellationToken cancellationToken)
        {
            var roles = await GetUserPermissionsForGroupAsync(userId, slug, cancellationToken);

            return roles is not null && roles.Any(x => x == action);
        }

        private async Task<IEnumerable<string>> GetSitePermissionsForRoles(List<string>? userRoles)
        {
            var permissions = new List<string>();

            if (userRoles != null && userRoles.Any())
            {
                foreach (var role in userRoles)
                {
                    var permissionsForRole = await _permissionsDataProvider.GetSitePermissionsForRole(role);

                    permissions.AddRange(permissionsForRole);
                }
            }
            else
            {
                var permissionsForRole = await _permissionsDataProvider.GetSitePermissionsForRole(GuestRole);
                permissions.AddRange(permissionsForRole);
            }

            return permissions.Distinct();
        }

        private async Task<IEnumerable<string>> GetSiteUserPermissionsForGroupRoles(IEnumerable<string>? userRoles)
        {
            var permissions = new List<string>();

            if (userRoles != null && userRoles.Any())
            {
                foreach (var role in userRoles)
                {
                    var permissionsForRole = await _permissionsDataProvider.GetGroupPermissionsForSiteRole(role);

                    permissions.AddRange(permissionsForRole);
                }
            }
            else
            {
                var permissionsForRole = await _permissionsDataProvider.GetGroupPermissionsForSiteRole(GuestRole);
                permissions.AddRange(permissionsForRole);
            }

            return permissions.Distinct();
        }


        private async Task<IEnumerable<string>> GetUserPermissionsForGroupRoles(IEnumerable<GroupUserRole>? userGroupRoles, Guid groupId, bool isPublicGroup)
        {
            var permissions = new List<string>();

            if (userGroupRoles is not null && userGroupRoles.Any())
            {
                // Check whether the user has been Approved, Banned, Pending approval etc
                var groupUserStatus = GetUserStatus(userGroupRoles.FirstOrDefault());

                // If user approved continue to work out their permissions
                if (groupUserStatus == Approved)
                {
                    foreach (var role in userGroupRoles)
                    {
                        var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(role.RoleName, groupId);
                        permissions.AddRange(permissionsForRole);
                    }
                }
                else if (groupUserStatus == PendingApproval)
                {
                    return permissions;
                }
                // If user not approved then set their role to guest
                else
                {
                    if (!isPublicGroup)
                    {
                        permissions.AddRange(await _permissionsDataProvider.GetPermissionsForGroupRole(string.Empty, groupId));
                        return permissions;
                    }

                    var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(GuestRole, groupId);

                    permissions.AddRange(permissionsForRole);
                }
            }
            else
            {
                if (!isPublicGroup)
                {
                    permissions.AddRange(await _permissionsDataProvider.GetPermissionsForGroupRole(string.Empty, groupId));
                    return permissions;
                }

                var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(GuestRole, groupId);

                permissions.AddRange(permissionsForRole);
            }

            return permissions;
        }

        private async Task<IEnumerable<string>> GetUserPermissionsForGroupRoles(IEnumerable<GroupUserRole>? userGroupRoles, string slug, bool isPublicGroup)
        {
            var permissions = new List<string>();

            if (userGroupRoles is not null && userGroupRoles.Any())
            {
                // Check whether the user has been Approved, Banned, Pending approval etc
                var groupUserStatus = GetUserStatus(userGroupRoles.FirstOrDefault());

                // If user approved continue to work out their permissions
                if (groupUserStatus == Approved)
                {
                    foreach (var role in userGroupRoles)
                    {
                        var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(role.RoleName, slug);
                        permissions.AddRange(permissionsForRole);
                    }
                }
                else if (groupUserStatus == PendingApproval)
                {
                    return permissions;
                }
                // If user not approved then set their role to guest
                else
                {
                    if (!isPublicGroup)
                    {
                        permissions.AddRange(await _permissionsDataProvider.GetPermissionsForGroupRole(string.Empty, slug));
                        return permissions;
                    }

                    var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(GuestRole, slug);

                    permissions.AddRange(permissionsForRole);
                }
            }
            else
            {
                if (!isPublicGroup)
                {
                    permissions.AddRange(await _permissionsDataProvider.GetPermissionsForGroupRole(string.Empty, slug));
                    return permissions;
                }

                var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(GuestRole, slug);

                permissions.AddRange(permissionsForRole);
            }

            return permissions;
        }

        private string GetUserStatus(GroupUserRole? groupUserRole)
        {
            if (groupUserRole is null) throw new ArgumentNullException(nameof(groupUserRole));

            if (groupUserRole.Approved && !groupUserRole.Banned && !groupUserRole.Locked && !groupUserRole.Rejected)
                return Approved;
            if (groupUserRole.Banned)
                return Banned;
            if (groupUserRole.Locked)
                return Locked;
            if (groupUserRole.Rejected)
                return Rejected;

            return PendingApproval;
        }
    }
}
