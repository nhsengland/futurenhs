//-----------------------------------------------------------------------
// <copyright file="FolderController.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using MvcForum.Core.Constants;
using MvcForum.Core.Interfaces;

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
        private readonly IFeatureManager _featureManager;

        public FolderController(IFolderRepository folderRepository, IFeatureManager featureManager)
        {
            _folderRepository = folderRepository;
            _featureManager = featureManager;
        }

        [ChildActionOnly]
        public PartialViewResult GetFolder(string slug, Guid folderId)
        {
            if (_featureManager.IsEnabled(Features.FilesAndFolders))
            {
                var model = new FolderListViewModel
                {
                    Slug = slug, Folder = _folderRepository.GetFolder(folderId).Result,
                    ChildFolders = _folderRepository.GetChildFolders(folderId).Result
                };

                return PartialView("_Folders", model);
            }

            return null;
        }
    }
}