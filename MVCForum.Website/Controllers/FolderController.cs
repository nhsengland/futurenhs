//-----------------------------------------------------------------------
// <copyright file="FolderController.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using MvcForum.Core.Repositories.Models;
using MvcForum.Web.ViewModels.Folder;

namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Constants;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.FilesAndFolders;
    using System;
    using System.Web.Mvc;

    public class FolderController : Controller
    {
        private readonly IFolderService _folderService;
        private readonly IFeatureManager _featureManager;
        private readonly IMembershipService _membershipService;

        public FolderController(IFolderService folderService, IFeatureManager featureManager, IMembershipService membershipService)
        {
            _folderService = folderService;
            _featureManager = featureManager;
            _membershipService = membershipService;
        }

        [HttpGet]
        public ViewResult CreateFolder(string slug, Guid? parentId, Guid groupId)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                var WriteFolder = new FolderWriteViewModel
                {
                    ParentFolder = parentId,
                    Slug = slug,
                    ParentGroup = groupId,
                    BreadCrumbTrail = BuildBreadCrumbTrail(parentId, slug)
            };
                return View("_CreateFolder", WriteFolder);
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFolder(FolderWriteViewModel folder)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                if (ModelState.IsValid) {
                    folder.AddedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                    var newId = _folderService.CreateFolder(folder);

                    return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = newId });
                }
                return View("_CreateFolder", folder);
            }

            return null;
        }

        [HttpGet]
        public ViewResult UpdateFolder(string slug, Guid? folderId)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                var result = _folderService.GetFolder(slug, folderId);

                var WriteFolder = new FolderWriteViewModel
                {
                    FolderId = folderId,
                    Slug = slug,
                    FolderName =  result.Folder.FolderName,
                    Description = result.Folder.Description,
                    BreadCrumbTrail = BuildBreadCrumbTrail(folderId, slug)
                };

                return View("_UpdateFolder", WriteFolder);
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateFolder(FolderWriteViewModel folder)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                if (ModelState.IsValid)
                {
                    var result = _folderService.GetFolder(folder.Slug, folder.FolderId);

                    if (folder.FolderId == result.Folder.FolderId)
                    {
                        folder.IsDeleted = folder.IsDeleted;
                        _folderService.UpdateFolder(folder);

                        return RedirectToRoute("GroupUrls", new {slug = folder.Slug, tab = Constants.GroupFilesTab, folder = folder.FolderId});
                    }
                }

                folder.BreadCrumbTrail = BuildBreadCrumbTrail(folder.FolderId, folder.Slug);
                return View("_UpdateFolder", folder);
            }

            return null;
        }

        [HttpGet]
        public ViewResult DeleteFolder(string slug, Guid? folderId)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                var result = _folderService.GetFolder(slug, folderId);

                var WriteFolder = new FolderWriteViewModel
                {
                    FolderId = folderId,
                    Slug = slug,
                    FolderName = result.Folder.FolderName,
                    Description = result.Folder.Description,
                    BreadCrumbTrail = BuildBreadCrumbTrail(folderId, slug)
                };

                return View("_DeleteFolder", WriteFolder);
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFolder(FolderWriteViewModel folder)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                if (ModelState.IsValid)
                {
                    var result = _folderService.GetFolder(folder.Slug, folder.FolderId);

                    if (folder.FolderId == result.Folder.FolderId)
                    {
                        folder.IsDeleted = true;
                        _folderService.UpdateFolder(folder);

                        return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = folder.ParentFolder });
                    }
                }
                return View("_DeleteFolder", folder);
            }

            return null;
        }

        [ChildActionOnly]
        public PartialViewResult GetFolder(string slug, Guid? folderId, Guid groupId)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                var model = _folderService.GetFolder(slug, folderId);
                model.GroupId = groupId;
                model.IsAdmin = _folderService.UserIsAdmin(slug, _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true)?.Id);
                model.BreadCrumbTrail = BuildBreadCrumbTrail(folderId, slug);

                return PartialView("_Folders", model);
            }

            return null;
        }

        private IEnumerable<BreadCrumbItem> BuildBreadCrumbTrail(Guid? folderId, string slug)
        {
            var list = new List<BreadCrumbItem> { new BreadCrumbItem { Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab }), Name = "Files" }};
            if (folderId.HasValue)
            {
                var bc = _folderService.GenerateBreadcrumbTrail(folderId.Value);
                list.AddRange(bc.Select(b => new BreadCrumbItem {Name = b.Name, Url = @Url.RouteUrl("GroupUrls", new {slug = slug, tab = Constants.GroupFilesTab, folder = b.Id})}));
            }

            return list;
        }
    }
}