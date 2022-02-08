using FutureNHS.Api.DataAccess.Models.Permissions;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using System.Security.Claims;

namespace FutureNHS.Api.Services
{
    public class PermissionsService : IPermissionsService
    {
        private const string GuestRole = "Guest";
        private const string Approved = "approved";
        private const string Banned = "banned";
        private const string Locked = "locked";
        private const string Rejected = "rejected";
        private const string PendingApproval = "pending";

        private readonly IPermissionsDataProvider _permissionsDataProvider;
        private readonly IRolesDataProvider _roleDataProvider;
        private readonly ILogger<PermissionsService> _logger;

        public PermissionsService(IRolesDataProvider roleDataProvider,IPermissionsDataProvider permissionsDataProvider, ILogger<PermissionsService> logger)
        {
            _permissionsDataProvider = permissionsDataProvider ?? throw new ArgumentNullException(nameof(permissionsDataProvider));
            _roleDataProvider = roleDataProvider ?? throw new ArgumentNullException(nameof(roleDataProvider));
            _logger = logger;
        }

        public async Task<IEnumerable<string>?> GetUserPermissionsForGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Cannot be EMPTY", nameof(userId));
            if (groupId == Guid.Empty) throw new ArgumentException("Cannot be EMPTY", nameof(groupId));

            cancellationToken.ThrowIfCancellationRequested();

            var (userRoles, groupUserRoles) = await _roleDataProvider.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken);

            if (userRoles is null || !userRoles.Any())
            {
                return null;
            }

            var permissions = new List<string>();

            permissions.AddRange(await GetSiteUserPermissionsForGroupRoles(userRoles));
            permissions.AddRange(await GetUserPermissionsForGroupRoles(groupUserRoles, groupId));
            
            return permissions.Distinct();
        }

        public async Task<IEnumerable<string>?> GetUserPermissionsAsync(Guid userId,CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Cannot be EMPTY", nameof(userId));

            cancellationToken.ThrowIfCancellationRequested();

            var userRoles = await _roleDataProvider.GetUserRolesAsync(userId,  cancellationToken);

            if (userRoles is null || !userRoles.Any())
            {
                return null;
            }

            var permissions = await GetSitePermissionsForRoles(userRoles);

            return permissions.Distinct();
        }

        private async Task<IEnumerable<string>> GetSitePermissionsForRoles(IEnumerable<string>? userRoles)
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


        private async Task<IEnumerable<string>> GetUserPermissionsForGroupRoles(IEnumerable<GroupUserRole>? userGroupRoles,Guid groupId)
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
                // If user not approved then set their role to guest
                else
                {
                    var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(GuestRole, groupId);

                    permissions.AddRange(permissionsForRole);                
                }
            }
            else
            {
                var permissionsForRole = await _permissionsDataProvider.GetPermissionsForGroupRole(GuestRole, groupId);

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
