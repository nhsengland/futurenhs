namespace MvcForum.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Constants;
    using Interfaces;
    using Interfaces.Services;
    using Models.Entities;
    using Utilities;

    public partial class PermissionService : IPermissionService
    {
        private IMvcForumContext _context;
        private readonly IGroupPermissionForRoleService _GroupPermissionForRoleService;
        private readonly ICacheService _cacheService;

        public PermissionService(IGroupPermissionForRoleService GroupPermissionForRoleService, IMvcForumContext context, ICacheService cacheService)
        {
            _GroupPermissionForRoleService = GroupPermissionForRoleService;
            _cacheService = cacheService;
            _context = context;
        }

        /// <inheritdoc />
        public void RefreshContext(IMvcForumContext context)
        {
            _context = context;
            _GroupPermissionForRoleService.RefreshContext(context);
        }

        /// <inheritdoc />
        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Permission> GetAll()
        {
            // Request Cache these as it gets called quite a lot
            var allPermissions = HttpContext.Current.Items["AllPermissions"];
            if (allPermissions == null)
            {
                HttpContext.Current.Items["AllPermissions"] = _context.Permission
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .ToList();
            }
            return (IEnumerable<Permission>)HttpContext.Current.Items["AllPermissions"];
        }

        /// <summary>
        /// Add a new permission
        /// </summary>
        /// <param name="permission"></param>
        public Permission Add(Permission permission)
        {
            permission.Name = StringUtils.SafePlainText(permission.Name);
            return _context.Permission.Add(permission);
        }

        /// <summary>
        ///  Delete permission and associated Group permission for roles
        /// </summary>
        /// <param name="permission"></param>
        public void Delete(Permission permission)
        {
            var catPermForRoles = _GroupPermissionForRoleService.GetByPermission(permission.Id);
            foreach (var GroupPermissionForRole in catPermForRoles)
            {
                _GroupPermissionForRoleService.Delete(GroupPermissionForRole);
            }

            _context.Permission.Remove(permission);
        }

        /// <summary>
        /// Get a permision by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Permission Get(Guid id)
        {

            return _context.Permission.FirstOrDefault(x => x.Id == id);
        }
    }
}
