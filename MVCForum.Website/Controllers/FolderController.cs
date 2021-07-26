//-----------------------------------------------------------------------
// <copyright file="FolderController.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using MvcForum.Web.ViewModels.Folder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class FolderController : Controller
    {
        private readonly IFolderRepository _folderRepository;

        public FolderController(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [ChildActionOnly]
        public PartialViewResult GetFolder(string slug, Guid folderId)
        {
            var model = new FolderListViewModel {Slug = slug,Folder = _folderRepository.GetFolder(folderId).Result, ChildFolders = _folderRepository.GetChildFolders(folderId).Result};

            return PartialView("_Folders", model);
        }
    }
}