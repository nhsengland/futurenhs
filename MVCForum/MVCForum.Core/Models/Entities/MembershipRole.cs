namespace MvcForum.Core.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Utilities;

    public partial class MembershipRole : IBaseEntity
    {
        public MembershipRole()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public virtual IList<MembershipUser> Users { get; set; }
        public virtual Settings Settings { get; set; }

        // Group Permissions
        public virtual IList<GroupPermissionForRole> GroupPermissionForRoles { get; set; }

        // Global Permissions
        public virtual IList<GlobalPermissionForRole> GlobalPermissionForRole { get; set; }

        public virtual Dictionary<Guid, Dictionary<Guid, bool>> GetGroupPermissionTable()
        {
            var permissionRows = new Dictionary<Guid, Dictionary<Guid, bool>>();

            foreach (var catPermissionForRole in GroupPermissionForRoles)
            {
                if (!permissionRows.ContainsKey(catPermissionForRole.Group.Id))
                {
                    var permissionList = new Dictionary<Guid, bool>();

                    permissionRows.Add(catPermissionForRole.Group.Id, permissionList);
                }

                if (!permissionRows[catPermissionForRole.Group.Id].ContainsKey(catPermissionForRole.Permission.Id))
                {
                    permissionRows[catPermissionForRole.Group.Id].Add(catPermissionForRole.Permission.Id, catPermissionForRole.IsTicked);
                }
                else
                {
                    permissionRows[catPermissionForRole.Group.Id][catPermissionForRole.Permission.Id] = catPermissionForRole.IsTicked;
                }
                

            }
            return permissionRows;
        }

    }
}
