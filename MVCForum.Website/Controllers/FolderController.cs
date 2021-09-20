﻿//-----------------------------------------------------------------------
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
                    Breadcrumbs = GetBreadcrumbs(parentId, slug, "Create folder")
                };
                ViewBag.HideSideBar = true;
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
                if (ModelState.IsValid) 
                {
                    folder.AddedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                    var newId = _folderService.CreateFolder(folder);

                    return RedirectToRoute("GroupUrls", new { slug = folder.Slug, tab = Constants.GroupFilesTab, folder = newId });
                }
                ViewBag.HideSideBar = true;
                folder.Breadcrumbs = GetBreadcrumbs(folder.FolderId, folder.Slug, "Create folder");

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
                    Breadcrumbs = GetBreadcrumbs(folderId, slug)
                };
                ViewBag.HideSideBar = true;
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
                ViewBag.HideSideBar = true;
                folder.Breadcrumbs = GetBreadcrumbs(folder.FolderId, folder.Slug);
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
                    Breadcrumbs = GetBreadcrumbs(folderId, slug)
                };
                ViewBag.HideSideBar = true;
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
                ViewBag.HideSideBar = true;
                folder.Breadcrumbs = GetBreadcrumbs(folder.FolderId, folder.Slug);
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
                model.Breadcrumbs = GetBreadcrumbs(folderId, slug);

                return PartialView("_Folders", model);
            }

            return null;
        }

        /// <summary>
        /// Generate the breadcrumbs for the folder view/create.
        /// </summary>
        /// <param name="folderId">Folder Id to get folder heirarchy.</param>
        /// <param name="slug"></param>
        /// <returns></returns>
        private BreadcrumbsViewModel GetBreadcrumbs(Guid? folderId, string slug, string lastEntry = null)
        {
            var breadCrumbs = new BreadcrumbsViewModel() { BreadcrumbLinks = new List<BreadCrumbItem>() };

            if (folderId.HasValue)
            {
                // Only add the root item if there is a folder Id, if it's null then we're at the root folder and no need to show breadcrumbs
                breadCrumbs.BreadcrumbLinks.Add(new BreadCrumbItem() { Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab }), Name = "Files" });

                var bc = _folderService.GenerateBreadcrumbTrail(folderId.Value);

                if(bc != null)
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
            }
            return breadCrumbs;
        }
    }
}