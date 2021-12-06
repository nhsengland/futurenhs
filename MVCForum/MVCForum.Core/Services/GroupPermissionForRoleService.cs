namespace MvcForum.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Constants;
    using Interfaces;
    using Interfaces.Services;
    using Models.Entities;

    public partial class GroupPermissionForRoleService : IGroupPermissionForRoleService
    {
        private IMvcForumContext _context;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheService"></param>
        public GroupPermissionForRoleService(IMvcForumContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context;
        }

        /// <inheritdoc />
        public void RefreshContext(IMvcForumContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Add new Group permission for role
        /// </summary>
        /// <param name="GroupPermissionForRole"></param>
        public GroupPermissionForRole Add(GroupPermissionForRole GroupPermissionForRole)
        {
            return _context.GroupPermissionForRole.Add(GroupPermissionForRole);
        }

        /// <summary>
        /// Check the Group permission for role actually exists
        /// </summary>
        /// <param name="GroupPermissionForRole"></param>
        /// <returns></returns>
        public GroupPermissionForRole CheckExists(GroupPermissionForRole GroupPermissionForRole)
        {
 
                if (GroupPermissionForRole.Permission != null &&
                    GroupPermissionForRole.Group != null &&
                    GroupPermissionForRole.MembershipRole != null)
                {

                    return _context.GroupPermissionForRole
                            .Include(x => x.MembershipRole)
                            .Include(x => x.Group)
                            .FirstOrDefault(x => x.Group.Id == GroupPermissionForRole.Group.Id &&
                                                 x.Permission.Id == GroupPermissionForRole.Permission.Id &&
                                                 x.MembershipRole.Id == GroupPermissionForRole.MembershipRole.Id);
                }

                return null;
         
        }

        /// <summary>
        /// Either updates a CPFR if exists or creates a new one
        /// </summary>
        /// <param name="GroupPermissionForRole"></param>
        public void UpdateOrCreateNew(GroupPermissionForRole GroupPermissionForRole)
        {
            // Firstly see if this exists already
            var permission = CheckExists(GroupPermissionForRole);

            // if it exists then just update it
            if (permission != null)
            {
                permission.IsTicked = GroupPermissionForRole.IsTicked;
            }
            else
            {
                Add(GroupPermissionForRole);
            }
        }


        /// <summary>
        /// Returns a row with the permission and CPFR
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cat"></param>
        /// <returns></returns>
        public Dictionary<Permission, GroupPermissionForRole> GetGroupRow(MembershipRole role, Group cat)
        {
                var catRowList = _context.GroupPermissionForRole
                .Include(x => x.MembershipRole)
                .Include(x => x.Group)
                .Include(x => x.Permission)
                .AsNoTracking()
                .Where(x => x.Group.Id == cat.Id &&
                            x.MembershipRole.Id == role.Id)
                            .ToList();
                return catRowList.ToDictionary(catRow => catRow.Permission);
        }

        /// <summary>
        /// Get all Group permissions by Group
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public IEnumerable<GroupPermissionForRole> GetByGroup(Guid GroupId)
        {
                return _context.GroupPermissionForRole
                                .Include(x => x.MembershipRole)
                                .Include(x => x.Group)
                                .Include(x => x.Permission)
                                .Where(x => x.Group.Id == GroupId)
                                .ToList();
        }

        public IEnumerable<GroupPermissionForRole> GetByRole(Guid roleId)
        {
                return _context.GroupPermissionForRole
                    .Include(x => x.MembershipRole)
                    .Include(x => x.Group)
                    .Include(x => x.Permission)
                    .Where(x => x.MembershipRole.Id == roleId);
        }

        public IEnumerable<GroupPermissionForRole> GetByPermission(Guid permId)
        {

                return _context.GroupPermissionForRole
                    .Include(x => x.MembershipRole)
                    .Include(x => x.Group)
                    .Include(x => x.Permission)
                    .Where(x => x.Permission.Id == permId);

        }

        public GroupPermissionForRole Get(Guid id)
        {

                return _context.GroupPermissionForRole
                        .Include(x => x.MembershipRole)
                        .Include(x => x.Group)
                        .Include(x => x.Permission)
                        .FirstOrDefault(cat => cat.Id == id);
      

        }

        public void Delete(GroupPermissionForRole GroupPermissionForRole)
        {
            _context.GroupPermissionForRole.Remove(GroupPermissionForRole);
        }
    }
}
