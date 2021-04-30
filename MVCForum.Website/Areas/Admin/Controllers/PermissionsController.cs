namespace MvcForum.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using Web.ViewModels;
    using Web.ViewModels.Admin;
    using Web.ViewModels.Group;

    [Authorize(Roles = Constants.AdminRoleName)]
    public class PermissionsController : BaseAdminController
    {
        private readonly IGroupPermissionForRoleService _GroupPermissionForRoleService;
        private readonly IGroupService _GroupService;
        private readonly IGlobalPermissionForRoleService _globalPermissionForRoleService;
        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;

        public PermissionsController(ILoggingService loggingService, IRoleService roleService,
            ILocalizationService localizationService, IPermissionService permissionService,
            IGroupService GroupService, IGroupPermissionForRoleService GroupPermissionForRoleService,
            IMembershipService membershipService, ISettingsService settingsService,
            IGlobalPermissionForRoleService globalPermissionForRoleService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, settingsService, context)
        {
            _roleService = roleService;
            _permissionService = permissionService;
            _GroupService = GroupService;
            _GroupPermissionForRoleService = GroupPermissionForRoleService;
            _globalPermissionForRoleService = globalPermissionForRoleService;
        }

        /// <summary>
        ///     List of roles to apply permissions to
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var permViewModel = new ChoosePermissionsViewModel
            {
                MembershipRoles = _roleService.AllRoles().ToList(),
                Permissions = _permissionService.GetAll().ToList()
            };
            return View(permViewModel);
        }

        /// <summary>
        ///     Add / Remove permissions for a role on each Group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditPermissions(Guid id)
        {
            var role = _roleService.GetRole(id);
            var permViewModel = new EditPermissionsViewModel
            {
                MembershipRole = role,
                Permissions = _permissionService.GetAll().ToList(),
                Groups = _GroupService.GetAll(),
                CurrentGlobalPermissions = _roleService.GetPermissions(null, role)
            };

            return View(permViewModel);
        }

        public ActionResult EditGroupPermissions(Guid id)
        {
            var Group = _GroupService.Get(id);
            var catPermissionViewModel = new EditGroupPermissionsViewModel
            {
                Group = Group,
                Permissions = _permissionService.GetAll().ToList(),
                Roles = _roleService.AllRoles()
                    .Where(x => x.RoleName != Constants.AdminRoleName)
                    .OrderBy(x => x.RoleName)
                    .ToList()
            };
            return View(catPermissionViewModel);
        }

        public ActionResult PermissionTypes()
        {
            var permViewModel = new ChoosePermissionsViewModel
            {
                Permissions = _permissionService.GetAll().ToList()
            };
            return View(permViewModel);
        }

        /// <summary>
        ///     Add a new permission type into the permission table
        /// </summary>
        /// <returns></returns>
        public ActionResult AddType()
        {
            return View(new AddTypeViewModel());
        }

        [HttpPost]
        public ActionResult AddType(AddTypeViewModel permissionViewModel)
        {
            try
            {
                var permission = new Permission
                {
                    Name = permissionViewModel.Name,
                    IsGlobal = permissionViewModel.IsGlobal
                };

                _permissionService.Add(permission);
                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Permission Added",
                    MessageType = GenericMessages.success
                };
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                throw;
            }


            return RedirectToAction("Index");
        }

        [HttpPost]
        public void UpdatePermission(AjaxEditPermissionViewModel ajaxEditPermissionViewModel)
        {
            try
            {
                if (Request.IsAjaxRequest())
                {
                    if (ajaxEditPermissionViewModel.Group == Guid.Empty)
                    {
                        // If Group is empty guid then this is a global permission

                        var gpr = new GlobalPermissionForRole
                        {
                            MembershipRole =
                                _roleService.GetRole(ajaxEditPermissionViewModel.MembershipRole),
                            Permission =
                                _permissionService.Get(ajaxEditPermissionViewModel.Permission),
                            IsTicked = ajaxEditPermissionViewModel.HasPermission
                        };
                        _globalPermissionForRoleService.UpdateOrCreateNew(gpr);
                    }
                    else
                    {
                        // We have a Group so it's a Group permission 

                        var mappedItem = new GroupPermissionForRole
                        {
                            Group = _GroupService.Get(ajaxEditPermissionViewModel.Group),
                            MembershipRole =
                                _roleService.GetRole(ajaxEditPermissionViewModel.MembershipRole),
                            Permission =
                                _permissionService.Get(ajaxEditPermissionViewModel.Permission),
                            IsTicked = ajaxEditPermissionViewModel.HasPermission
                        };
                        _GroupPermissionForRoleService.UpdateOrCreateNew(mappedItem);
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                throw;
            }
        }

        public ActionResult DeletePermission(Guid id)
        {
            try
            {
                var permission = _permissionService.Get(id);
                _permissionService.Delete(permission);

                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Permission Deleted",
                    MessageType = GenericMessages.success
                };
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                throw;
            }

            return RedirectToAction("Index");
        }
    }
}