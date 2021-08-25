using MvcForum.Core.Repositories.Models;

namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.FilesAndFolders;
    using System;
    using System.Web.Mvc;
    using MvcForum.Core.Constants;
    using System.Collections.Generic;
    using System.Linq;
    using Status = Core.Models.Enums.UploadStatus;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// Defines methods and routes for the GroupFIles.
    /// </summary>
    [Authorize]
    public class GroupFileController : AsyncController
    {
        /// <summary>
        /// Instance of the <see cref="IFileService"/>.
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// Instance of the <see cref="IMembershipService"/>.
        /// </summary>
        private readonly IMembershipService _membershipService;

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
            if (fileService is null) { throw new ArgumentNullException(nameof(fileService)); }
            if (membershipService is null) { throw new ArgumentNullException(nameof(membershipService)); }
            if (folderService is null) { throw new ArgumentNullException(nameof(folderService)); }
            if (localizationService is null) { throw new ArgumentNullException(nameof(localizationService)); }

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
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Show")]
        public async Task<ActionResult> ShowAsync(Guid id, string slug, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) { throw new ArgumentOutOfRangeException(nameof(id)); }

            // TODO - Check user has permissions to view file

            var dbFile = await _fileService.GetFileAsync(id, cancellationToken);

            var file = new FileViewModel
            {
                File = dbFile,
                Slug = slug,
                Breadcrumbs = GetBreadcrumbs(dbFile.ParentFolder, slug, dbFile.Title)
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
            if (folderId == Guid.Empty) { throw new ArgumentOutOfRangeException(nameof(folderId)); }

            var viewmodel = new FileWriteViewModel
            {
                FolderId = folderId,
                Slug = slug,
                Breadcrumbs = GetBreadcrumbs(folderId, slug, "Upload file")
            };
            ViewBag.HideSideBar = true;
            return View(viewmodel);
        }

        /// <summary>
        /// Method to accept the posted create file form and process the creation of the file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The detail view for the new file.</returns>
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync(FileWriteViewModel file, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ModelState.IsValid)
            {
                if(file is null)
                {
                    throw new ArgumentNullException(nameof(file), "The required file parameter is missing.");
                }

                // Check that the folder Id passed in is valid (folder exists).
                if (!(await FolderIsValidAsync(file.FolderId, cancellationToken)))
                {
                    ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("File.Error.InvalidFolder"));
                }
                else
                {
                    // Perform full validation prior to actual upload attempt
                    var fileValidation = _fileService.FileValidation(file.PostedFile);

                    // Simple validation passed, attempt upload and save to DB
                    if (!fileValidation.ValidationErrors.Any())
                    {
                        file.CreatedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                        var id = _fileService.Create(file);

                        // Only continue if file meta data added to DB
                        if (id != Guid.Empty)
                        {
                            file.FileId = id;

                            // Validate and attempt to upload the file to blob storage
                            var fileUploadResult = await _fileService.UploadFileAsync(file.PostedFile, fileValidation.MimeType, cancellationToken);

                            // Update the status of the file depending on upload result.
                            file.UploadStatus = fileUploadResult.UploadSuccessful ? (int)Status.Verified : (int)Status.Failed;
                            file.FileUrl = fileUploadResult.UploadSuccessful ? fileUploadResult.UploadedFileName : null;
                            file.FileName = file.PostedFile.FileName;
                            file.FileSize = file.PostedFile.ContentLength;
                            file.FileExtension = System.IO.Path.GetExtension(file.PostedFile.FileName);
                            file.BlobHash = fileUploadResult.UploadedFileHash;

                            _fileService.Update(file);

                            if (fileUploadResult.UploadSuccessful)
                            {
                                // Success, show the file
                                return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
                            }
                            else
                            {
                                // Not successful, throw an exception
                                throw new ApplicationException("Error uploading file to storage.");
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
                        foreach (var error in fileValidation.ValidationErrors)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                    }
                }
            }
            ViewBag.HideSideBar = true;
            file.Breadcrumbs = GetBreadcrumbs(file.FolderId, file.Slug, "Upload file");
            return View(file);
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Update")]
        public async Task<ActionResult> UpdateAsync(Guid id, string slug, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) { throw new ArgumentOutOfRangeException(nameof(id)); }

            var file = await _fileService.GetFileAsync(id, cancellationToken);

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
                if (file is null) { throw new ArgumentNullException(nameof(file)); }

                file.ModifiedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;
                _fileService.Update(file);

                return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
            }

            return View();
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(Guid id, string slug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == Guid.Empty) { throw new ArgumentOutOfRangeException(nameof(id)); }

            var file = await _fileService.GetFileAsync(id, cancellationToken);

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
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            _fileService.Delete(file);
            return RedirectToRoute("GroupUrls", new { slug = file.Slug, tab = Constants.GroupFilesTab, folder = file.FolderId });
        }

        /// <summary>
        /// Download the file from blob storage.
        /// </summary>
        /// <param name="fileId">Id of the file to download.</param>
        /// <param name="slug">Group slug for redirect if an error.</param>
        /// <returns></returns>
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Download")]
        public async Task<ActionResult> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default(CancellationToken))
        {
            //TODO - Check user has permissions to download file

            if (fileId == Guid.Empty)
            {
                throw new InvalidOperationException("Unable to download file as the Id is not valid");
            }

            // Get the file by Id passed in
            var fileModel = await _fileService.GetFileAsync(fileId, cancellationToken);

            if (string.IsNullOrWhiteSpace(fileModel?.FileUrl))
            {
                throw new InvalidOperationException("The requested file does not have a valid name");
            }

            // Ensure the file is of status verified
            if (fileModel.Status == (int)Status.Verified)
            {
                // File valid for Id, try and get the link to the blob
                var downloadPath = await _fileService.GetRelativeDownloadUrlAsync(fileModel.FileUrl, Azure.Storage.Sas.BlobSasPermissions.Read, cancellationToken);

                if (string.IsNullOrWhiteSpace(downloadPath))
                {
                    throw new InvalidOperationException("Unable to download file, the end point is not valid");
                }

                // Append download path to files path and redirect
                return Redirect($"/gateway/media/{downloadPath}");
            }
            else
            {
                throw new InvalidOperationException("The requested file is not valid");
            }
        }

        /// <summary>
        /// Verify that the Folder Id is for a valid folder (folder exists).
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        private async Task<bool> FolderIsValidAsync(Guid folderId, CancellationToken cancellationToken)
        {
            return (await _folderService.GetFolderAsync(string.Empty, folderId, cancellationToken)).Folder != null;
        }

        /// <summary>
        /// Generate the breadcrumbs for the file view/upload.
        /// </summary>
        /// <param name="folderId">Folder Id to get parent folder heirarchy.</param>
        /// <param name="slug"></param>
        /// <param name="filename">Name of file to append as non clickable text</param>
        /// <returns></returns>
        private BreadcrumbsViewModel GetBreadcrumbs(Guid? folderId, string slug, string filename = null)
        {
            var breadCrumbs = new BreadcrumbsViewModel() { BreadcrumbLinks = new List<BreadCrumbItem>() };
            breadCrumbs.BreadcrumbLinks.Add(new BreadCrumbItem() { Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab }), Name = "Files" });
            if (folderId.HasValue)
            {
                var bc = _folderService.GenerateBreadcrumbTrail(folderId.Value);

                if (bc != null)
                {
                    breadCrumbs.BreadcrumbLinks.AddRange(bc.Select(b => new BreadCrumbItem { Name = b.Name, Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab, folder = b.Id }) }));
                }
            }
            breadCrumbs.LastEntry = filename;

            return breadCrumbs;
        }
    }
}
