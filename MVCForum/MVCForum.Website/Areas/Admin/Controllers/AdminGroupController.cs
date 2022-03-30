namespace MvcForum.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using ExtensionMethods;
    using ViewModels;
    using ViewModels.Admin;

    [Authorize(Roles = Constants.AdminRoleName)]
    public class AdminGroupController : BaseAdminController
    {
        private readonly IGroupService _groupService;
        protected new MembershipUser LoggedOnReadOnlyUser;
        public AdminGroupController(ILoggingService loggingService,
            IMvcForumContext context,
            IMembershipService membershipService,
            ILocalizationService localizationService,
            IGroupService GroupService,
            ISettingsService settingsService)
            : base(loggingService, membershipService, localizationService, settingsService, context)
        {
            _groupService = GroupService;
            LoggedOnReadOnlyUser = membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult GetMainGroups()
        {

            var viewModel = new ListGroupsViewModel
            {
                Groups = _groupService.GetAll(LoggedOnReadOnlyUser?.Id).OrderBy(x => x.SortOrder)
            };
            return PartialView(viewModel);
        }

        /// <summary>
        ///     Removes the Group image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveGroupImage(Guid id)
        {
            var Group = _groupService.Get(id);
            Group.Image = string.Empty;
            Context.SaveChanges();
            return RedirectToAction("EditGroup", new {id});
        }

        public ActionResult CreateGroup()
        {
            var GroupViewModel = new GroupEditViewModel
            {
                AllGroups = _groupService.GetBaseSelectListGroups(_groupService.GetAll(LoggedOnReadOnlyUser?.Id), LoggedOnReadOnlyUser?.Id),
                AllSections = _groupService.GetAllSections().ToSelectList(),
				Users = MembershipService.GetAll().ToSelectList(),
                Public = true
            };
            return View(GroupViewModel);
        }

        /// <summary>
        ///     Create Group logic
        /// </summary>
        /// <param name="GroupViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CreateGroup")]
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public async Task<ActionResult> CreateGroupAsync(GroupEditViewModel GroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var Group = GroupViewModel.ToGroup();

                Group.GroupOwner = MembershipService.GetUser(GroupViewModel.GroupOwner);

                var GroupResult = await _groupService.Create(Group, GroupViewModel.Files,
                    GroupViewModel.ParentGroup, GroupViewModel.Section);
                if (!GroupResult.Successful)
                {
                    ModelState.AddModelError("", GroupResult.ProcessLog.FirstOrDefault());
                }
                else
                {
                    if(GroupViewModel.GroupAdministrators != null && GroupViewModel.GroupAdministrators.Any())
                        _groupService.AddGroupAdministrators(Group.Slug, GroupViewModel.GroupAdministrators.ToList(),LoggedOnReadOnlyUser.Id);
                    // We use temp data because we are doing a redirect
                    TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Group Created",
                        MessageType =
                            GenericMessages.success
                    };
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating the Group");
            }

            GroupViewModel.AllGroups = _groupService.GetBaseSelectListGroups(_groupService.GetAll(LoggedOnReadOnlyUser?.Id), LoggedOnReadOnlyUser?.Id);
            GroupViewModel.AllSections = _groupService.GetAllSections().ToSelectList();
            GroupViewModel.Users = MembershipService.GetAll().ToSelectList();

            return View(GroupViewModel);
        }

        public ActionResult EditGroup(Guid id)
        {
            var group = _groupService.Get(id);
            var groupViewModel = group.ToEditViewModel(_groupService.GetBaseSelectListGroups(_groupService.GetAll(LoggedOnReadOnlyUser?.Id).Where(x => x.Id != group.Id).ToList(), LoggedOnReadOnlyUser?.Id), _groupService.GetAllSections().ToSelectList());
            groupViewModel.RowVersion = group.RowVersion;
            groupViewModel.Users = MembershipService.GetAll().ToSelectList();
            return View(groupViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditGroup")]
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public async Task<ActionResult> EditGroupAsync(GroupEditViewModel GroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var administrators = GroupViewModel.GroupAdministrators;
                //_groupService.getg
                // Reset the select list
                GroupViewModel.AllGroups = _groupService.GetBaseSelectListGroups(_groupService.GetAll(LoggedOnReadOnlyUser?.Id)

                    .Where(x => x.Id != GroupViewModel.Id)
                    .ToList(), LoggedOnReadOnlyUser?.Id);

                var groupToEdit = _groupService.Get(GroupViewModel.Id);

                var group = GroupViewModel.ToGroup(groupToEdit);
                group.RowVersion = GroupViewModel.RowVersion;
                var groupResult = await _groupService.Edit(group, GroupViewModel.Files,
                    GroupViewModel.ParentGroup, GroupViewModel.Section);
                if (!groupResult.Successful)
                {
                    ModelState.AddModelError("", groupResult.ProcessLog.FirstOrDefault());
                }
                else
                {
                    // We use temp data because we are doing a redirect
                    TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Group Edited",
                        MessageType = GenericMessages.success
                    };

                    // Set the view model
                    GroupViewModel = groupResult.EntityToProcess.ToEditViewModel(
                        _groupService.GetBaseSelectListGroups(_groupService.GetAll(LoggedOnReadOnlyUser?.Id)
                            .Where(x => x.Id != GroupViewModel.Id)
                            .ToList(), LoggedOnReadOnlyUser?.Id), _groupService.GetAllSections().ToSelectList());

                    if (administrators != null && administrators.Any())
                    {
              
                        _groupService.AddGroupAdministrators(group.Slug, administrators.Where(x=> x != Guid.Empty).ToList(), LoggedOnReadOnlyUser.Id);
                    }
                    else
                    {
                        _groupService.AddGroupAdministrators(group.Slug, new List<Guid>(), LoggedOnReadOnlyUser.Id);
                    }
                    return RedirectToAction("Index");
                }
            }
            GroupViewModel.Users = MembershipService.GetAll().ToSelectList();

            return View(GroupViewModel);
        }

        public ActionResult DeleteGroupConfirmation(Guid id)
        {
            var cat = _groupService.Get(id);
            var subCats = _groupService.GetAllSubGroups(id, LoggedOnReadOnlyUser?.Id).ToList();
            var viewModel = new DeleteGroupViewModel
            {
                Id = cat.Id,
                Group = cat,
                SubGroups = subCats
            };

            return View(viewModel);
        }

        [ActionName("DeleteGroup")]
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public async Task<ActionResult> DeleteGroupAsync(Guid id)
        {
            var cat = _groupService.Get(id);
            var GroupResult = await _groupService.Delete(cat);

            if (!GroupResult.Successful)
            {
                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = GroupResult.ProcessLog.FirstOrDefault(),
                    MessageType = GenericMessages.danger
                };
            }
            else
            {
                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Group Deleted",
                    MessageType = GenericMessages.success
                };
            }

            return RedirectToAction("Index");
        }

        public ActionResult SyncGroupPaths()
        {
            try
            {
               
                // var all Groups
                var all = _groupService.GetAll(LoggedOnReadOnlyUser?.Id);

                // Get all the Groups
                var mainGroups = all.Where(x => x.Parent_GroupId == null).ToList();

                // Get the sub Groups
                var subGroups = all.Where(x => x.Parent_GroupId != null).ToList();

                // loop through the main Groups and get all it's sub Groups
                foreach (var mainGroup in mainGroups)
                {
                    // get a list of sub Groups, from this Group
                    var subCats = new List<Group>();
                    subCats = GetAllGroupSubGroups(mainGroup, subGroups, subCats);

                    // Now loop through these subGroups and set the paths
                    var count = 1;
                    var prevCatId = string.Empty;
                    var prevPath = string.Empty;
                    foreach (var cat in subCats)
                    {
                        if (count == 1)
                        {
                            // If first count just set the parent Group Id
                            cat.Path = mainGroup.Id.ToString();
                        }
                        else
                        {
                            // If past one, then we use the previous Group
                            if (string.IsNullOrWhiteSpace(prevPath))
                            {
                                cat.Path = prevCatId;
                            }
                            else
                            {
                                cat.Path = string.Concat(prevPath, ",", prevCatId);
                            }
                        }
                        prevCatId = cat.Id.ToString();
                        prevPath = cat.Path;
                        count++;
                    }

                    // Save changes on each Group
                    Context.SaveChanges();
                }


                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Group Paths Synced",
                    MessageType = GenericMessages.success
                };
                Context.SaveChanges();
            }
            catch (Exception)
            {
                Context.RollBack();
                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Error syncing paths",
                    MessageType = GenericMessages.danger
                };
            }

            return RedirectToAction("Index");
        }

        private static List<Group> GetAllGroupSubGroups(Group parent, List<Group> allSubGroups,
            List<Group> subCats)
        {
            foreach (var cat in allSubGroups)
            {
                if (cat.Parent_GroupId.Id == parent.Id)
                {
                    subCats.Add(cat);
                    GetAllGroupSubGroups(cat, allSubGroups, subCats);
                }
            }
            return subCats;
        }

        #region Sections

        /// <summary>
        /// Sections page
        /// </summary>
        /// <returns></returns>
        public ActionResult Sections()
        {
            return View();
        }

        /// <summary>
        /// List sections
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult GetSections()
        {
            var viewModel = new SectionListViewModel
            {
                Sections = _groupService.GetAllSections()
            };
            return PartialView(viewModel);
        }

        /// <summary>
        /// Create edit section view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddEditSection(Guid? id)
        {
            var GroupViewModel = new SectionAddEditViewModel();

            if (id != null)
            {
                var section = _groupService.GetSection(id.Value);

                GroupViewModel.IsEdit = true;
                GroupViewModel.Id = section.Id;
                GroupViewModel.Name = section.Name;
                GroupViewModel.Description = section.Description;
                GroupViewModel.SortOrder = section.SortOrder;
            }

            return View(GroupViewModel);
        }

        /// <summary>
        ///     Create / Edit a section logic
        /// </summary>
        /// <param name="sectionAddEditViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditSection(SectionAddEditViewModel sectionAddEditViewModel)
        {
            if (ModelState.IsValid)
            {
                var section = sectionAddEditViewModel.IsEdit ? _groupService.GetSection(sectionAddEditViewModel.Id) 
                                                                : new Section{DateCreated = DateTime.UtcNow};

                section.Name = sectionAddEditViewModel.Name;
                section.Description = sectionAddEditViewModel.Description;
                section.SortOrder = sectionAddEditViewModel.SortOrder;

                // TODO - This should all be in the service!!!
                if (!sectionAddEditViewModel.IsEdit)
                {
                    Context.Section.Add(section);
                }

                Context.SaveChanges();


                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Successful",
                    MessageType = GenericMessages.success
                };

                return RedirectToAction("Sections");
            }

            return View(sectionAddEditViewModel);
        }

        public ActionResult DeleteSection(Guid id)
        {
            _groupService.DeleteSection(id);

            TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = "Successful",
                MessageType = GenericMessages.success
            };

            return RedirectToAction("Sections");
        }

        #endregion

    }
}