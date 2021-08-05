namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.FilesAndFolders;
    using System;
    using System.Web.Mvc;
    using MvcForum.Core.Constants;
    using MvcForum.Web.ViewModels.Shared;
    using System.Collections.Generic;
    using MvcForum.Core.Constants.UI;

    /// <summary>
    /// Defines methods and routes for the GroupFIles.
    /// </summary>
    public class GroupFileController : Controller
    {
        /// <summary>
        /// Instance of the <see cref="IFileService"/>.
        /// </summary>
        private IFileService _fileService { get; set; }

        private IMembershipService _membershipService { get; set; }

        /// <summary>
        /// Constructs a new instance of the <see cref="GroupFileController"/>.
        /// </summary>
        /// <param name="fileRepository"></param>
        public GroupFileController(IFileService fileService, IMembershipService membershipService)
        {
            _fileService = fileService;
            _membershipService = membershipService;
        }

        /// <summary>
        /// Method to return the details view for a given file Id.
        /// </summary>
        /// <param name="id">Id of the file to show.</param>
        /// <returns>The show view.</returns>
        public ActionResult Show(Guid id, string slug)
        {
            FileViewModel file = new FileViewModel
            {
                File = _fileService.GetFile(id),
                Slug = slug
            };

            return View(file);
        }

        /// <summary>
        /// Method to display the create file view.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public ActionResult Create(Guid folderId, Guid groupId, string slug)
        {
            var viewmodel = new FileWriteViewModel
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(FileWriteViewModel file)
        {
            if (ModelState.IsValid)
            {
                file.CreatedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                Guid id = _fileService.Create(file);

                return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
            }

            return View();
        }

        public ActionResult Update(Guid id, string slug)
        {
            var file = _fileService.GetFile(id);

            var viewModel = new FileWriteViewModel
            {
                FileId = file.Id,
                Name = file.Title,
                Description = file.Description,
                FolderId = file.ParentFolder,
                Slug = slug
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(FileWriteViewModel file)
        {
            if (ModelState.IsValid)
            {
                file.ModifiedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                Guid id = _fileService.Update(file);

                return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
            }

            return View();
        }

        public ActionResult Delete(Guid id, string slug)
        {
            var file = _fileService.GetFile(id);

            var viewModel = new FileWriteViewModel
            {
                FileId = file.Id,
                Name = file.Title,
                Description = file.Description,
                FolderId = file.ParentFolder,
                Slug = slug
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FileWriteViewModel file)
        {
            _fileService.Delete(file);
            return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
        }
    }
}
