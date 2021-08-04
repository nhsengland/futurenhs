﻿namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.FilesAndFolders;
    using System;
    using System.Web.Mvc;
    using MvcForum.Core.Constants;

    /// <summary>
    /// Defines methods and routes for the GroupFIles.
    /// </summary>
    public class GroupFileController : Controller
    {
        /// <summary>
        /// Instance of the <see cref="IFileService"/>.
        /// </summary>
        private IFileService _fileService { get; set; }

        /// <summary>
        /// Constructs a new instance of the <see cref="GroupFileController"/>.
        /// </summary>
        /// <param name="fileRepository"></param>
        public GroupFileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Method to return the details view for a given file Id.
        /// </summary>
        /// <param name="id">Id of the file to show.</param>
        /// <returns>The show view.</returns>
        public ActionResult Show(Guid id)
        {
            var file = _fileService.GetFile(id);

            return View(file);
        }

        /// <summary>
        /// Method to display the create file view.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public ActionResult Create(Guid folderId, string slug)
        {
            var viewmodel = new CreateGroupFileViewModel
            {
                FolderId = folderId,
                Slug = slug
            };

            return View(viewmodel);
        }

        /// <summary>
        /// Method to accept the posted create file form and process the creation of the file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The detail view for the new file.</returns>
        [HttpPost]
        public ActionResult Create(CreateGroupFileViewModel file)
        {
            if (ModelState.IsValid)
            {
                Guid id = _fileService.Create(file);

                return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
            }

            return View();
        }
    }
}
