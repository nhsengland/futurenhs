using MvcForum.Core.Repositories.Models;

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
    using System.Threading.Tasks;
    using System.Linq;
    using Status = MvcForum.Core.Models.Enums.UploadStatus;
    using System.Configuration;

    /// <summary>
    /// Defines methods and routes for the GroupFIles.
    /// </summary>
    public class GroupFileController : AsyncController
    {
        /// <summary>
        /// Instance of the <see cref="IFileService"/>.
        /// </summary>
        private IFileService _fileService { get; set; }

        /// <summary>
        /// Instance of the <see cref="IMembershipService"/>.
        /// </summary>
        private IMembershipService _membershipService { get; set; }

        /// <summary>
        /// Instance of the <see cref="ILocalizationService"/>.
        /// </summary>
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Instance of the <see cref="IFolderService"/>.
        /// </summary>
        private readonly IFolderService _folderService;

        /// <summary>
        /// Constructs a new instance of the <see cref="GroupFileController"/>.
        /// </summary>
        /// <param name="fileService"></param>
        /// <param name="membershipService"></param>
        /// <param name="folderService"></param>
        /// <param name="localizationService"></param>
        public GroupFileController(IFileService fileService, IMembershipService membershipService, IFolderService folderService, ILocalizationService localizationService)
        {
            _fileService = fileService;
            _membershipService = membershipService;
            _folderService = folderService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Method to return the details view for a given file Id.
        /// </summary>
        /// <param name="id">Id of the file to show.</param>
        /// <returns>The show view.</returns>
        public ActionResult Show(Guid id, string slug)
        {
            var dbFile = _fileService.GetFile(id);

            var file = new FileViewModel
            {
                File = dbFile,
                Slug = slug,
                BreadCrumbTrail = BuildBreadCrumbTrail(dbFile.ParentFolder, slug)
            };
            ViewBag.HideSideBar = true;
            return View(file);
        }

        /// <summary>
        /// Method to display the create file view.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public ActionResult Create(Guid folderId, string slug)
        {
            var viewmodel = new FileWriteViewModel
            {
                FolderId = folderId,
                Slug = slug,
                BreadCrumbTrail = BuildBreadCrumbTrail(folderId, slug)
            };
            ViewBag.HideSideBar = true;
            return View(viewmodel);
        }

        /// <summary>
        /// Method to accept the posted create file form and process the creation of the file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The detail view for the new file.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FileWriteViewModel file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check that the folder Id passed in is valid (folder exists).
                    if (!FolderIsValid(file.FolderId))
                    {
                        ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("File.Error.InvalidFolder"));
                    }
                    else
                    {
                        // Perform simple validation prior to actual upload attempt
                        var simpleValidation = _fileService.SimpleFileValidation(file.PostedFile);

                        // Simple validation passed, attempt upload and save to DB
                        if (!simpleValidation.ValidationErrors.Any())
                        {
                            file.CreatedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                            var id = _fileService.Create(file);

                            // Only continue if file meta data added to DB
                            if (id != Guid.Empty)
                            {
                                file.FileId = id;

                                // Validate and attempt to upload the file to blob storage
                                var fileUploadResult = await _fileService.UploadFileAsync(file.PostedFile);

                                // Update the status of the file depending on upload result.
                                file.UploadStatus = fileUploadResult.UploadSuccessful ? (int)Status.Uploaded : (int)Status.Failed;
                                file.FileUrl = fileUploadResult.UploadSuccessful ? fileUploadResult.UploadedFileName : null;
                                file.FileName = file.PostedFile.FileName;
                                file.FileSize = file.PostedFile.ContentLength.ToString();
                                file.FileExtension = System.IO.Path.GetExtension(file.PostedFile.FileName);

                                _fileService.Update(file);

                                if (fileUploadResult.UploadSuccessful)
                                {
                                    // Success, show the file
                                    return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
                                }
                                else
                                {
                                    // Not successful surface the errors
                                    foreach(var error in fileUploadResult.ValidationErrors)
                                    {
                                        ModelState.AddModelError(string.Empty, error);
                                    }
                                }
                            }
                            else
                            {
                                // Error saving file data surface the error.
                                ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("File.Error.FileSaveError"));
                            }
                        }
                        else
                        {
                            // Not successful surface the errors
                            foreach (var error in simpleValidation.ValidationErrors)
                            {
                                ModelState.AddModelError(string.Empty, error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO - deal with any unhandled exceptions.
                }
            }
            ViewBag.HideSideBar = true;
            file.BreadCrumbTrail = BuildBreadCrumbTrail(file.FolderId, file.Slug);
            return View(file);
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

        /// <summary>
        /// Verify that the Folder Id is for a valid folder.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        private bool FolderIsValid(Guid folderId)
        {
            return _folderService.GetFolder(string.Empty, folderId).Folder != null;
        }

        private IEnumerable<BreadCrumbItem> BuildBreadCrumbTrail(Guid? folderId, string slug)
        {
            var list = new List<BreadCrumbItem> { new BreadCrumbItem { Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab }), Name = "Files" } };
            if (folderId.HasValue)
            {
                var bc = _folderService.GenerateBreadcrumbTrail(folderId.Value);
                list.AddRange(bc.Select(b => new BreadCrumbItem { Name = b.Name, Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab, folder = b.Id }) }));
            }

            return list;
        }
    }
}
