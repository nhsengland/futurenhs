namespace MvcForum.Core.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using Models.Entities;

    public partial interface IGroupPermissionForRoleService : IContextService
    {
        /// <summary>
        ///     Add new Group permission for role
        /// </summary>
        /// <param name="GroupPermissionForRole"></param>
        GroupPermissionForRole Add(GroupPermissionForRole GroupPermissionForRole);

        /// <summary>
        ///     Check the Group permission for role actually exists
        /// </summary>
        /// <param name="GroupPermissionForRole"></param>
        /// <returns></returns>
        GroupPermissionForRole CheckExists(GroupPermissionForRole GroupPermissionForRole);

        /// <summary>
        ///     Either updates a CPFR if exists or creates a new one
        /// </summary>
        /// <param name="GroupPermissionForRole"></param>
        void UpdateOrCreateNew(GroupPermissionForRole GroupPermissionForRole);

        /// <summary>
        ///     Returns a row with the permission and CPFR
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cat"></param>
        /// <returns></returns>
        Dictionary<Permission, GroupPermissionForRole> GetGroupRow(MembershipRole role, Group cat);

        /// <summary>
        ///     Get all Group permissions by Group
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        IEnumerable<GroupPermissionForRole> GetByGroup(Guid GroupId);

        IEnumerable<GroupPermissionForRole> GetByRole(Guid roleId);
        IEnumerable<GroupPermissionForRole> GetByPermission(Guid permId);
        GroupPermissionForRole Get(Guid id);
        void Delete(GroupPermissionForRole GroupPermissionForRole);
    }
}