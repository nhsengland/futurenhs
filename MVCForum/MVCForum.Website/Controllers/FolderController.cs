namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Constants;
    using MvcForum.Core.ExtensionMethods;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Enums;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Web.ViewModels.Breadcrumb;
    using MvcForum.Web.ViewModels.Folder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize]
    public sealed class FolderController : Controller
    {
        private readonly IFolderService _folderService;
        private readonly IFeatureManager _featureManager;
        private readonly IMembershipService _membershipService;
        private readonly ILocalizationService _localizationService;
        private readonly IGroupService _groupService;

        public FolderController(IFolderService folderService, IFeatureManager featureManager, IMembershipService membershipService, ILocalizationService localizationService, IGroupService groupService)
        {
            _folderService = folderService;
            _featureManager = featureManager;
            _membershipService = membershipService;
            _localizationService = localizationService;
            _groupService = groupService;
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("CreateFolder")]
        [HttpGet]
        public async Task<ActionResult> CreateFolderAsync(string slug, Guid? parentId, Guid groupId, CancellationToken cancellationToken)
        {
            if (!_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!await _folderService.IsUserAdminAsync(slug, _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id, cancellationToken))
            {
                return RedirectToRoute("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab, folder = parentId });
            }

            var WriteFolder = new FolderWriteViewModel
            {
                ParentFolder = parentId,
                Slug = slug,
                ParentGroup = groupId,
                Breadcrumbs = await GetBreadcrumbsAsync(parentId, slug, "Create folder", cancellationToken)
            };

            ViewBag.HideSideBar = true;
            return View("_CreateFolder", WriteFolder);           
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("CreateFolder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateFolderAsync(FolderWriteViewModel folder, CancellationToken cancellationToken)
        {
            if (!_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!await _folderService.IsUserAdminAsync(folder.Slug, _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id, cancellationToken))
            {
                return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = folder.ParentFolder });
            }

            if (ModelState.IsValid) 
            {
                if (await _folderService.IsFolderNameValidAsync(folder.FolderId, folder.FolderName, folder.ParentFolder, folder.ParentGroup, cancellationToken))
                {
                    folder.AddedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                    var newId = _folderService.CreateFolder(folder);

                    return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = newId });
                }
                ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("Folder.Error.DuplicateTitle"));
            }

            ViewBag.HideSideBar = true;
            folder.Breadcrumbs = await GetBreadcrumbsAsync(folder.FolderId, folder.Slug, "Create folder", cancellationToken);

            return View("_CreateFolder", folder);
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("UpdateFolder")]
        [HttpGet]
        public async Task<ActionResult> UpdateFolderAsync(string slug, Guid groupId, Guid folderId, Guid? parentId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _folderService.GetFolderAsync(slug, folderId, cancellationToken);

            if (!await _folderService.UserHasFolderWriteAccessAsync(result.Folder.FolderId, _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id, cancellationToken))
            {
                return RedirectToRoute("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab, folder = folderId });
            }

            var WriteFolder = new FolderWriteViewModel
            {
                FolderId = result.Folder.FolderId,
                Slug = result.Slug,
                FolderName = result.Folder.FolderName,
                OriginalFolderName = result.Folder.FolderName,
                Description = result.Folder.Description,
                Breadcrumbs = await GetBreadcrumbsAsync(result.Folder.FolderId, result.Slug, cancellationToken: cancellationToken),
                ParentGroup = groupId,
                ParentFolder = result.Folder.ParentId
            };

            ViewBag.HideSideBar = true;
            ViewData.Add("FolderName", WriteFolder.FolderName);
            return View("_UpdateFolder", WriteFolder);            
           
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("UpdateFolder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateFolderAsync(FolderWriteViewModel folder, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                return RedirectToAction("Index", "Home");
            }            

            if (ModelState.IsValid)
            {
                var result = await _folderService.GetFolderAsync(folder.Slug, folder.FolderId, cancellationToken);

                if (!await _folderService.UserHasFolderWriteAccessAsync(result.Folder.FolderId, _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id, cancellationToken))
                {
                    return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = folder.ParentFolder });
                }

                // Check folder exists for folder Id passed in
                if (folder.FolderId == result.Folder.FolderId)
                {
                    // Ensure folder name is valid
                    if (await _folderService.IsFolderNameValidAsync(folder.FolderId, folder.FolderName, folder.ParentFolder, folder.ParentGroup, cancellationToken))
                    {
                        folder.IsDeleted = folder.IsDeleted;
                        _folderService.UpdateFolder(folder);

                        return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = folder.FolderId });
                    }
                    ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("Folder.Error.DuplicateTitle"));
                }
            }

            ViewBag.HideSideBar = true;
            folder.Breadcrumbs = await GetBreadcrumbsAsync(folder.FolderId, folder.Slug, cancellationToken: cancellationToken);
            return View("_UpdateFolder", folder);            
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("DeleteFolder")]
        [HttpGet]
        public async Task<ActionResult> DeleteFolderAsync(FolderWriteViewModel folder, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _folderService.GetFolderAsync(folder.Slug, folder.FolderId, cancellationToken);
            if (folder.FolderId == result.Folder.FolderId)
            {
                if (!await _folderService.UserHasFolderWriteAccessAsync(result.Folder.FolderId, _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id, cancellationToken))
                {
                    return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = folder.FolderId, hasError = true });
                }
            
                if (await _folderService.DeleteFolderAsync(result.Folder.FolderId, cancellationToken))
                {
                    return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = result.Folder.ParentId });
                }
            }

            return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = folder.FolderId, hasError = true });
        }

        [ChildActionOnly]
        public PartialViewResult GetFolder(string slug, Guid? folderId, Guid groupId, bool hasError = false)
        {
            if (!_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                return null;
            }
            //needed to stop blocking, PartialViewResuls don't support async in MVC5
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);

            var currentUser = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);            

            var model = _folderService.GetFolderAsync(slug, folderId, CancellationToken.None).Result;

            if (!_folderService.UserHasFolderReadAccessAsync(slug, currentUser.Id, CancellationToken.None).Result)
            {
                return null;
            }

            var groupUserStatus = _groupService.GetAllForUser(currentUser.Id).FirstOrDefault(x => x.Group.Id == groupId).GetUserStatusForGroup();

            model.GroupId = groupId;
            model.IsAdmin = _folderService.IsUserAdminAsync(slug, currentUser.Id, CancellationToken.None).Result;
            model.Breadcrumbs = GetBreadcrumbsAsync(folderId, slug, cancellationToken: CancellationToken.None).Result;
            model.GroupUserStatus = groupUserStatus;
            model.IsMember = groupUserStatus == GroupUserStatus.Joined;
            model.HasError = hasError;        

            SynchronizationContext.SetSynchronizationContext(syncContext);
            return PartialView("_Folders", model);            
        }

        private async Task<BreadcrumbsViewModel> GetBreadcrumbsAsync(Guid? folderId, string slug, string lastEntry = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var breadCrumbs = new BreadcrumbsViewModel() { BreadcrumbLinks = new List<BreadCrumbItem>() };

            if (!folderId.HasValue)
            {
                return breadCrumbs;
            }
            // Only add the root item if there is a folder Id, if it's null then we're at the root folder and no need to show breadcrumbs
            breadCrumbs.BreadcrumbLinks.Add(new BreadCrumbItem() { Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab }), Name = "Files" });

            var bc = await _folderService.GenerateBreadcrumbTrailAsync(folderId.Value, cancellationToken);

            if (!(bc is null))
            {
                if (!string.IsNullOrWhiteSpace(lastEntry))
                {
                    // Last entry not passed in use all the folder heirarchy as links with last entry - add only
                    breadCrumbs.BreadcrumbLinks.AddRange(bc.Select(b => new BreadCrumbItem { Name = b.Name, Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab, folder = b.Id }) }));
                    breadCrumbs.LastEntry = lastEntry;
                }
                else
                {
                    // Exclude the last folder as this is the folder being viewed and should not be clickable, add last folder as non clickable
                    breadCrumbs.BreadcrumbLinks.AddRange(bc.Take(bc.Count() - 1).Select(b => new BreadCrumbItem { Name = b.Name, Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab, folder = b.Id }) }));
                    breadCrumbs.LastEntry = bc.Last().Name;
                }
            }
            
            return breadCrumbs;
        }
    }
}