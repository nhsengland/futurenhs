namespace MvcForum.Core.Models.General
{
    using System.Collections.Generic;
    using Entities;

    public partial class PermissionSet : Dictionary<string, PermissionForRole>
    {
        public PermissionSet(IEnumerable<GroupPermissionForRole> GroupPermissions, IEnumerable<GlobalPermissionForRole> globalPermissions)
        {
            // Add the Group permissions
            foreach (var GroupPermissionForRole in GroupPermissions)
            {
                Add(GroupPermissionForRole.Permission.Name, MapGroupPermission(GroupPermissionForRole));
            }

            // Add the global permissions
            foreach (var globalPermissionForRole in globalPermissions)
            {
                Add(globalPermissionForRole.Permission.Name, MapGlobalPermission(globalPermissionForRole));
            }
        }

        private static PermissionForRole MapGroupPermission(GroupPermissionForRole cp)
        {
            var pfr = new PermissionForRole
            {
                Group = cp.Group,
                IsTicked = cp.IsTicked,
                MembershipRole = cp.MembershipRole,
                Permission = cp.Permission
            };

            return pfr;
        }

        private static PermissionForRole MapGlobalPermission(GlobalPermissionForRole gp)
        {
            var pfr = new PermissionForRole
            {
                IsTicked = gp.IsTicked,
                MembershipRole = gp.MembershipRole,
                Permission = gp.Permission
            };

            return pfr;
        }
    }
}
