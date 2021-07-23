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

        public ActionResult Index(Guid? parent = null)
        {
            var model = new FolderListViewModel();

            model.Folders = _folderRepository.GetFolders(parent).Result;

            return View(model);
        }
    }
}