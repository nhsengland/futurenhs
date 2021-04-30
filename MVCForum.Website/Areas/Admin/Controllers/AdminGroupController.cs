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
        private readonly IGroupService _GroupService;

        public AdminGroupController(ILoggingService loggingService,
            IMvcForumContext context,
            IMembershipService membershipService,
            ILocalizationService localizationService,
            IGroupService GroupService,
            ISettingsService settingsService)
            : base(loggingService, membershipService, localizationService, settingsService, context)
        {
            _GroupService = GroupService;
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
                Groups = _GroupService.GetAll().OrderBy(x => x.SortOrder)
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
            var Group = _GroupService.Get(id);
            Group.Image = string.Empty;
            Context.SaveChanges();
            return RedirectToAction("EditGroup", new {id});
        }

        public ActionResult CreateGroup()
        {
            var GroupViewModel = new GroupEditViewModel
            {
                AllGroups = _GroupService.GetBaseSelectListGroups(_GroupService.GetAll()),
                AllSections = _GroupService.GetAllSections().ToSelectList()
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
        public async Task<ActionResult> CreateGroup(GroupEditViewModel GroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var Group = GroupViewModel.ToGroup();

                var GroupResult = await _GroupService.Create(Group, GroupViewModel.Files,
                    GroupViewModel.ParentGroup, GroupViewModel.Section);
                if (!GroupResult.Successful)
                {
                    ModelState.AddModelError("", GroupResult.ProcessLog.FirstOrDefault());
                }
                else
                {
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

            GroupViewModel.AllGroups = _GroupService.GetBaseSelectListGroups(_GroupService.GetAll());
            GroupViewModel.AllSections = _GroupService.GetAllSections().ToSelectList();

            return View(GroupViewModel);
        }

        public ActionResult EditGroup(Guid id)
        {
            var Group = _GroupService.Get(id);
            var GroupViewModel = Group.ToEditViewModel(_GroupService.GetBaseSelectListGroups(_GroupService.GetAll().Where(x => x.Id != Group.Id).ToList()), _GroupService.GetAllSections().ToSelectList());

            return View(GroupViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditGroup(GroupEditViewModel GroupViewModel)
        {
            if (ModelState.IsValid)
            {
                // Reset the select list
                GroupViewModel.AllGroups = _GroupService.GetBaseSelectListGroups(_GroupService.GetAll()
                    .Where(x => x.Id != GroupViewModel.Id)
                    .ToList());

                var GroupToEdit = _GroupService.Get(GroupViewModel.Id);

                var Group = GroupViewModel.ToGroup(GroupToEdit);

                var GroupResult = await _GroupService.Edit(Group, GroupViewModel.Files,
                    GroupViewModel.ParentGroup, GroupViewModel.Section);
                if (!GroupResult.Successful)
                {
                    ModelState.AddModelError("", GroupResult.ProcessLog.FirstOrDefault());
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
                    GroupViewModel = GroupResult.EntityToProcess.ToEditViewModel(
                        _GroupService.GetBaseSelectListGroups(_GroupService.GetAll()
                            .Where(x => x.Id != GroupViewModel.Id)
                            .ToList()), _GroupService.GetAllSections().ToSelectList());
                }
            }

            return View(GroupViewModel);
        }

        public ActionResult DeleteGroupConfirmation(Guid id)
        {
            var cat = _GroupService.Get(id);
            var subCats = _GroupService.GetAllSubGroups(id).ToList();
            var viewModel = new DeleteGroupViewModel
            {
                Id = cat.Id,
                Group = cat,
                SubGroups = subCats
            };

            return View(viewModel);
        }

        public async Task<ActionResult> DeleteGroup(Guid id)
        {
            var cat = _GroupService.Get(id);
            var GroupResult = await _GroupService.Delete(cat);

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
                var all = _GroupService.GetAll();

                // Get all the Groups
                var mainGroups = all.Where(x => x.ParentGroup == null).ToList();

                // Get the sub Groups
                var subGroups = all.Where(x => x.ParentGroup != null).ToList();

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
                if (cat.ParentGroup.Id == parent.Id)
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
                Sections = _GroupService.GetAllSections()
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
                var section = _GroupService.GetSection(id.Value);

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
                var section = sectionAddEditViewModel.IsEdit ? _GroupService.GetSection(sectionAddEditViewModel.Id) 
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
            _GroupService.DeleteSection(id);

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