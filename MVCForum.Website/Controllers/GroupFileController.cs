using System.Web.Http.Results;
using MvcForum.Core.ExtensionMethods;
using MvcForum.Core.Interfaces.Providers;
using MvcForum.Core.Repositories.Models;
using MvcForum.Core.Services;

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
    using System.Web.SessionState;
    using MvcForum.Web.ViewModels.Folder;

    [Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public sealed class GroupFileController : AsyncController
    {

        private readonly IFileService _fileService;

        private readonly IMembershipService _membershipService;

        private readonly ILocalizationService _localizationService;

        private readonly IFolderService _folderService;

        private readonly IFileServerService _fileServerService;

        private readonly IConfigurationProvider _configurationProvider;

        /// <summary>
        /// Constructs a new instance of the <see cref="GroupFileController"/>.
        /// </summary>
        /// <param name="fileService"></param>
        /// <param name="membershipService"></param>
        /// <param name="folderService"></param>
        /// <param name="localizationService"></param>
        public GroupFileController(IFileService fileService, IMembershipService membershipService, IFolderService folderService, ILocalizationService localizationService, IFileServerService fileServerService, IConfigurationProvider configurationProvider)
        {
            if (fileService is null) { throw new ArgumentNullException(nameof(fileService)); }
            if (membershipService is null) { throw new ArgumentNullException(nameof(membershipService)); }
            if (folderService is null) { throw new ArgumentNullException(nameof(folderService)); }
            if (localizationService is null) { throw new ArgumentNullException(nameof(localizationService)); }
            if (fileServerService is null) { throw new ArgumentNullException(nameof(fileServerService)); }
            if (configurationProvider is null) { throw new ArgumentNullException(nameof(configurationProvider)); }

            _fileService = fileService;
            _membershipService = membershipService;
            _folderService = folderService;
            _localizationService = localizationService;
            _fileServerService = fileServerService;
            _configurationProvider = configurationProvider;
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
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id));

            if (!UserHasFileReadAccess(id))
            {
                return RedirectToAction("Index", "Home");
            }

            var dbFile = await _fileService.GetFileAsync(id, cancellationToken);

            if (dbFile is null) throw new ApplicationException("No file found for supplied Id");

            var file = new FileViewModel
            {
                File = dbFile,
                Slug = slug,
                Breadcrumbs = GetBreadcrumbs(dbFile.ParentFolder, slug, dbFile.Title),
                IsUpdatable = await UserHasFileUpdateAccessAsync(id, cancellationToken)
            };

            ViewBag.HideSideBar = true;
            return View(file);
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("LoadFilePreview")]
        [HttpGet]
        public async Task<PartialViewResult> LoadFilePreview(Guid id, string slug, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id));

            if (UserHasFileReadAccess(id))
            {
                var cookies = Request.GetCookieContainer(_configurationProvider.ApplicationGatewayFqdn);

                var response = await _fileServerService.GetCollaboraFileUrl(id, cookies, "view");
                if (response != null)
                {
                    return PartialView("_FilePreview", response);
                }
            }

            return null;
        }

        [ActionName("Create")]
        [AsyncTimeout(120000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public async Task<ActionResult> CreateAsync(Guid folderId, string groupSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!UserHasFileWriteAccess(folderId))
            {
                return RedirectToAction("Index", "Home");
            }

            if (folderId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(folderId));

            var folderModel = await _folderService.GetFolderAsync(groupSlug, folderId, CancellationToken.None);

            if (folderModel is null) throw new ApplicationException("No folder found for Id passed in");

            var viewmodel = new FileUploadViewModel
            {
                FolderName = folderModel.Folder.FolderName,
                Breadcrumbs = GetBreadcrumbs(folderId, groupSlug, "Upload file"),
                FileToUpload = new FileWriteViewModel
                {
                    FolderId = folderId,
                    Slug = groupSlug,
                }
            };

            ViewBag.HideSideBar = true;
            return View(viewmodel);
        }

        /// <summary>
        /// Method to accept the posted create file form and process the creation of the file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The detail view for the new file.</returns>
        [ActionName("Create")]
        [AsyncTimeout(120000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(FileUploadViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ModelState.IsValid)
            {
                if(model is null) throw new ArgumentNullException(nameof(model), "The required model parameter is missing.");

                // Set file to be uploaded from model
                var file = model.FileToUpload;

                if (file is null) throw new ApplicationException("File upload data is missing");

                if (!UserHasFileWriteAccess(file.FolderId))
                {
                    return RedirectToAction("Index", "Home");
                }

                // Check that the folder Id passed in is valid (folder exists).
                if (!FolderIsValid(file.FolderId))
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
            model.Breadcrumbs = GetBreadcrumbs(model.FileToUpload.FolderId, model.FileToUpload.Slug, "Upload file");
            return View(model);
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Update")]
        public async Task<ActionResult> UpdateAsync(Guid id, string slug, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id));

            var file = await _fileService.GetFileAsync(id, cancellationToken);

            if (file is null) throw new ApplicationException("No file found for update for supplied Id");

            if (!(await UserHasFileUpdateAccessAsync(file.Id, cancellationToken)))
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new FileUpdateViewModel
            {
                FileId = file.Id,
                Name = file.Title,
                Description = file.Description,
                FolderId = file.ParentFolder,
                GroupSlug = slug,
                Breadcrumbs = GetBreadcrumbs(file.ParentFolder, slug, "Edit file"),
            };

            return View(viewModel);
        }

        [HttpPost]
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Update")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAsync(Guid id, string slug, FileUpdateViewModel file, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Guid.Empty == id) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));
            if (file is null) throw new ArgumentNullException(nameof(file));

            var fileFromDb = await _fileService.GetFileAsync(id, cancellationToken);

            if (fileFromDb is null) throw new ApplicationException("No file found for update for supplied Id");

            if (ModelState.IsValid)
            {
                if (!(await UserHasFileUpdateAccessAsync(id, cancellationToken)))
                {
                    return RedirectToAction("Index", "Home");
                }

                file.FileId = id;
                file.ModifiedBy = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true).Id;

                if (await _fileService.UpdateAsync(file, cancellationToken))
                {
                    return RedirectToRoute("GroupFileUrls", new { slug = slug, tab = Constants.GroupFilesTab, id = id });
                }
            }

            file.GroupSlug = slug;
            file.Breadcrumbs = GetBreadcrumbs(fileFromDb.ParentFolder, slug, "Edit file");
            return View(file);
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(Guid id, string slug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id));            

            var file = await _fileService.GetFileAsync(id, cancellationToken);

            if (file is null) throw new ApplicationException("No file found for delete for supplied Id");

            if (!UserHasFileWriteAccess(file.ParentFolder))
            {
                return RedirectToAction("Index", "Home");
            }

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
            if (file is null) throw new ArgumentNullException(nameof(file));

            if (!UserHasFileWriteAccess(file.FolderId))
            {
                return RedirectToAction("Index", "Home");
            }

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
        public async Task<ActionResult> DownloadAsync(Guid fileId, string groupSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(fileId));

            // Check user has permissions to download file
            if (!UserHasFileReadAccess(fileId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Get the file by Id passed in
            var fileModel = await _fileService.GetFileAsync(fileId, cancellationToken);

            if (fileModel is null) throw new ApplicationException("No file found for download for supplied Id");

            if (string.IsNullOrWhiteSpace(fileModel?.BlobName)) throw new ApplicationException("The requested file does not have a valid name");

            // Ensure the file is of status verified
            if (fileModel.Status == (int)Status.Verified)
            {
                // File valid for Id, try and get the link to the blob
                var downloadPath = await _fileService.GetRelativeDownloadUrlAsync(fileModel.BlobName, Azure.Storage.Sas.BlobSasPermissions.Read, cancellationToken);

                if (string.IsNullOrWhiteSpace(downloadPath))
                {
                    throw new ApplicationException("Unable to download file, the end point is not valid");
                }

                // Append download path to files path and redirect
                return Redirect($"/gateway/media/{downloadPath}");
            }
            else
            {
                throw new ApplicationException("The requested file is not valid");
            }
        }

        /// <summary>
        /// Verify that the Folder Id is for a valid folder (folder exists).
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        private bool FolderIsValid(Guid folderId)
        {
            return _folderService.IsFolderIdValidAsync(folderId, CancellationToken.None).Result;
        }

        //private bool UserHasGroupAccess(string groupSlug)
        //{
        //    if (System.Web.HttpContext.Current.User is null) throw new NullReferenceException("User not logged in");

        //    var user = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);

        //    if (user is null) throw new ApplicationException("No user found for logged in Id");

        //    return _folderService.UserHasFolderReadAccessAsync(groupSlug, user.Id, CancellationToken.None).Result;
        //}

        private bool UserHasFileReadAccess(Guid fileId)
        {
            if (System.Web.HttpContext.Current.User is null) throw new NullReferenceException("User not logged in");

            var user = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);

            if (user is null) throw new ApplicationException("No user found for logged in Id");
            return _fileService.UserHasFileAccessAsync(fileId, user.Id, CancellationToken.None).Result;
        }


        private bool UserHasFileWriteAccess(Guid folderId)
        {
            if (System.Web.HttpContext.Current.User is null) throw new NullReferenceException("User not logged in");

            var user = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);

            if (user is null) throw new ApplicationException("No user found for logged in Id");
            return _folderService.UserHasFileWriteAccessAsync(folderId, user.Id, CancellationToken.None).Result;
        }

        private async Task<bool> UserHasFileUpdateAccessAsync(Guid fileId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Guid.Empty == fileId) throw new ArgumentOutOfRangeException(nameof(fileId));

            if (System.Web.HttpContext.Current.User is null) throw new NullReferenceException("User not logged in");

            var user = _membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);

            if (user is null) throw new ApplicationException("No user found for logged in Id");

            var file = await _fileService.GetFileAsync(fileId, cancellationToken);

            if (file is null) throw new ApplicationException("No file found for update for supplied Id");

            var loggedInUser = user.Id;

            // If has folder write access then either admin or group admin
            var hasFolderWriteAccess = await _folderService.UserHasFolderWriteAccessAsync(file.ParentFolder, user.Id, cancellationToken);

            return hasFolderWriteAccess || file.CreatedBy == loggedInUser;
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
            var x = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab });
            breadCrumbs.BreadcrumbLinks.Add(new BreadCrumbItem() { Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab }), Name = "Files" });
            if (folderId.HasValue)
            {
                var bc = _folderService.GenerateBreadcrumbTrailAsync(folderId.Value, CancellationToken.None).Result;

                if (bc != null)
                {
                    if (bc.Count() == 4)
                    {
                        bc.First().Name = "...";
                    }
                    breadCrumbs.BreadcrumbLinks.AddRange(bc.Select(b => new BreadCrumbItem { Name = b.Name, Url = @Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupFilesTab, folder = b.Id }) }));
                }
            }
            breadCrumbs.LastEntry = filename;

            return breadCrumbs;
        }
    }
}
