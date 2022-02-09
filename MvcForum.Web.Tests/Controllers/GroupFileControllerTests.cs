﻿using MvcForum.Core.Interfaces.Providers;

namespace MvcForum.Web.Tests.Controllers
{
    using Moq;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Web.Controllers;
    using MvcForum.Web.ViewModels.Folder;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    [TestFixture]
    public class GroupFileControllerTests
    {
        private Mock<IFileService> _fileService;

        private Mock<IMembershipService> _membershipService;

        private Mock<IFolderService> _folderService;

        private Mock<ILocalizationService> _localizationService;

        private Mock<IFileServerService> _fileServerService;

        private Mock<IConfigurationProvider> _configurationProvider;

        private Mock<HttpPostedFileBase> _postedFile;

        private GroupFileController _groupFileController;

        private Guid _fileId = Guid.NewGuid();
        private readonly Guid _folderId = Guid.NewGuid();
        private readonly Guid _addUpdateUserId = Guid.NewGuid();
        private readonly byte[] _fileUploadedFileHash = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        private readonly string _slug = "test-group";

        private readonly FileReadViewModel _fileReadViewModel = new FileReadViewModel();
        private readonly FileWriteViewModel _fileWriteViewModel = new FileWriteViewModel();
        private readonly FileUpdateViewModel _fileUpdateViewModel = new FileUpdateViewModel();
        private readonly FileUploadViewModel _fileUploadViewModel = new FileUploadViewModel();
        private readonly FolderReadViewModel _folderReadViewModel = new FolderReadViewModel();
        private readonly FolderViewModel _folderViewModel = new FolderViewModel();
        private readonly ValidateBlobResult _validateBlobResult = new ValidateBlobResult() { MimeType = "pdf" };
        private readonly UploadBlobResult _uploadBlobResult = new UploadBlobResult();

        private const string FileIdParameterName = "id";
        private const string FileIdFullParameterName = "fileId";
        private const string ModelParameterName = "model";
        private const string FileParameterName = "file";
        private const string SlugParameterName = "slug";

        private const string FolderIdParameterName = "folderId";

        private const string AddUpdateUserName = "testuser";
        private const string FileUploadedFileName = "Uploaded file name";
        private const string PostedFileActualFileName = "postedfile.pdf";
        private const string DownloadFileName = "downloadfile.pdf";
        private const int PostedFileContentLength = 123456;
        private const string RouteName = "GroupUrls";
        private const string FilesRouteName = "GroupFileUrls";
        private const string RouteUrl = "groups/{slug}/{tab}";
        private const string RouteController = "Group";
        private const string RouteAction = "Show";

        private const string ActionKey = "action";
        private const string ControllerKey = "controller";

        private const string NoAccessRedirectAction = "Index";
        private const string NoAccessRedirectController = "Home";

        // Errors
        private const string UserNotLoggedInError = "User not logged in";
        private const string UserNotFoundForIdError = "No user found for logged in Id";

        private const string FileNotFoundForIdError = "No file found for supplied Id";
        private const string FolderNotFoundForIdError = "No folder found for Id passed in";

        private const string FileUploadDataMissingError = "File upload data is missing";

        private const string UpdateFileNotFoundForIdError = "No file found for update for supplied Id";
        private const string DeleteFileNotFoundForIdError = "No file found for delete for supplied Id";
        private const string DownloadFileNotFoundForIdError = "No file found for download for supplied Id";
        private const string DownloadFileNoFileNameError = "The requested file does not have a valid name";
        private const string DownloadFileInvalidStatusError = "The requested file is not valid";
        private const string DownloadFileInvalidEndpointError = "Unable to download file, the end point is not valid";
        private const string StorageUploadError = "Error uploading file to storage.";
        private const string ErrorInvalidFolder = "Invalid folder";
        private const string ErrorSavingFileToDb = "Error saving to DB";
        private const string ValidateBlobError = "Error";

        private const string DownloadPathPrefix = "/gateway/media/";
        private const string DownloadLink = "downloadlink";

        [SetUp]
        public void Setup()
        {
            _fileService = new Mock<IFileService>();
            _membershipService = new Mock<IMembershipService>();
            _folderService = new Mock<IFolderService>();
            _localizationService = new Mock<ILocalizationService>();
            _fileServerService = new Mock<IFileServerService>();
            _configurationProvider = new Mock<IConfigurationProvider>();

        _postedFile = new Mock<HttpPostedFileBase>();

            _groupFileController = new GroupFileController(_fileService.Object, _membershipService.Object, _folderService.Object, _localizationService.Object, _fileServerService.Object, _configurationProvider.Object);

            // Need to reset each time
            _fileId = Guid.NewGuid();

            _fileWriteViewModel.FileId = _fileId;
            _fileWriteViewModel.FolderId = _folderId;

            _folderReadViewModel.FolderId = _folderId;

            _folderViewModel.Folder = _folderReadViewModel;

            _localizationService.Setup(x => x.GetResourceString("File.Error.InvalidFolder")).Returns(ErrorInvalidFolder);
            _localizationService.Setup(x => x.GetResourceString("File.Error.FileSaveError")).Returns(ErrorSavingFileToDb);

            // Reset any validation errors
            _validateBlobResult.ValidationErrors = new List<string>();

            SetContext();
        }

        // ===========================================================================================================================
        // ShowAsync

        [Test]
        public void ShowAsync_FileIdParameterEmpty_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFileController.ShowAsync(Guid.Empty, null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FileIdParameterName, response.ParamName);
        }

        [Test]
        public void ShowAsync_UserNotLoggedIn_ThrowsException()
        {
            var response = Assert.ThrowsAsync<NullReferenceException>(async () => await _groupFileController.ShowAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void ShowAsync_UserNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.ShowAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task ShowAsync_UserDoesNotHaveGroupAccess_ReturnsRedirectResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(false);

            var response = await _groupFileController.ShowAsync(_fileId, null, CancellationToken.None) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public void ShowAsync_FileNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileAccess(true);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.ShowAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(FileNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task ShowAsync_Success_ReturnsViewResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupController();
            SetupUserHasFileAccess(true);


            _fileService.Setup(x => x.GetFileAsync(_fileId, CancellationToken.None)).Returns(Task.FromResult(_fileReadViewModel));

            var response = await _groupFileController.ShowAsync(_fileId, null, CancellationToken.None) as ViewResult;

            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileViewModel>(response.Model);
        }

        // ===========================================================================================================================
        // Create Async Get
         
        [Test]
        public void Create_UserNotLoggedIn_ThrowsException()
        {
            var response = Assert.ThrowsAsync<NullReferenceException>(async delegate { await _groupFileController.CreateAsync(Guid.Empty, null); });

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void Create_UserNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);

            var response = Assert.ThrowsAsync<ApplicationException>(async delegate { await _groupFileController.CreateAsync(Guid.Empty, null); });

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task Create_UserDoesNotHaveGroupAccess_ReturnsRedirectResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(false);

            var response = (await _groupFileController.CreateAsync(Guid.Empty, null)) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public void Create_FolderIdParameterEmpty_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);

            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async delegate { await _groupFileController.CreateAsync(Guid.Empty, null); });

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FolderIdParameterName, response.ParamName);
        }

        [Test]
        public void Create_FolderNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);

            var response = Assert.ThrowsAsync<ApplicationException>( async delegate { await _groupFileController.CreateAsync(_folderId, null); });

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(FolderNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task Create_Success_ReturnsViewResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);
            SetupController();

            _folderService.Setup(x => x.GetFolderAsync(null, _folderId, CancellationToken.None)).Returns(Task.FromResult(_folderViewModel));

            var response = (await _groupFileController.CreateAsync(_folderId, null)) as ViewResult;

            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileUploadViewModel>(response.Model);
        }

        // ===========================================================================================================================
        // CreateAsync Post

        [Test]
        public void CreateAsync_ModelParameterNull_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupFileController.CreateAsync(null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(ModelParameterName, response.ParamName);
        }

        [Test]
        public void CreateAsync_FileUploadDataMissing_ThrowsException()
        {
            _fileUploadViewModel.FileToUpload = null;

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(FileUploadDataMissingError, response.Message);
        }

        [Test]
        public void CreateAsync_UserNotLoggedIn_ThrowsException()
        {
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            var response = Assert.ThrowsAsync<NullReferenceException>(async () => await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None));

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void CreateAsync_UserNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task CreateAsync_UserDoesNotHaveGroupAccess_ReturnsRedirectResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(false);
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            var response = await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public async Task CreateAsync_FolderNotValidForId_ReturnsInvalidFolderError()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);
            SetupController();
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            _folderService.Setup(x => x.GetFolderAsync(string.Empty, _folderId, CancellationToken.None)).Returns(Task.FromResult(_folderViewModel));

            var response = await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None) as ViewResult;

            var responseModel = response.Model;
            var modelStateIsValid = response.ViewData.ModelState.IsValid;
            var errorCount = response.ViewData.ModelState.Values.Select(x => x.Errors.Count).FirstOrDefault();
            var error = response.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;

            // Only expecting one error, if errorCount != 1 or first error is not as expected then test will fail.
            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileUploadViewModel>(responseModel);
            Assert.IsFalse(modelStateIsValid);
            Assert.AreEqual(1, errorCount);
            Assert.AreEqual(ErrorInvalidFolder, error);
        }

        [Test]
        public async Task CreateAsync_PostedFileFailsValidation_ReturnsValidateBlobError()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);
            SetupController();
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            _validateBlobResult.ValidationErrors = new List<string>() { ValidateBlobError };
            _fileService.Setup(x => x.FileValidation(It.IsAny<HttpPostedFileBase>())).Returns(_validateBlobResult);
            _folderService.Setup(x => x.GetFolderAsync(_slug, _folderId, CancellationToken.None)).Returns(Task.FromResult(_folderViewModel));
            _folderService.Setup(x => x.IsFolderIdValidAsync(_folderId, CancellationToken.None)).Returns(Task.FromResult(true));

            var response = await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None) as ViewResult;

            var responseModel = response.Model;
            var modelStateIsValid = response.ViewData.ModelState.IsValid;
            var errorCount = response.ViewData.ModelState.Values.Select(x => x.Errors.Count).FirstOrDefault();
            var error = response.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;

            // Only expecting one error, if errorCount != 1 or first error is not as expected then test will fail.
            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileUploadViewModel>(responseModel);
            Assert.IsFalse(modelStateIsValid);
            Assert.AreEqual(1, errorCount);
            Assert.AreEqual(ValidateBlobError, error);
        }

        [Test]
        public async Task CreateAsync_ErrorAddingFileToDb_ReturnsAddingToDbError()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);
            SetupController();
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            _fileService.Setup(x => x.FileValidation(It.IsAny<HttpPostedFileBase>())).Returns(_validateBlobResult);
            _fileService.Setup(x => x.Create(_fileWriteViewModel)).Returns(Guid.Empty);
            _folderService.Setup(x => x.GetFolderAsync(_slug, _folderId, CancellationToken.None)).Returns(Task.FromResult(_folderViewModel));
            _folderService.Setup(x => x.IsFolderIdValidAsync(_folderId, CancellationToken.None)).Returns(Task.FromResult(true));

            var response = await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None) as ViewResult;

            var responseModel = response.Model;
            var modelStateIsValid = response.ViewData.ModelState.IsValid;
            var errorCount = response.ViewData.ModelState.Values.Select(x => x.Errors.Count).FirstOrDefault();
            var error = response.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;

            // Only expecting one error, if errorCount != 1 or first error is not as expected then test will fail.
            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileUploadViewModel>(responseModel);
            Assert.IsFalse(modelStateIsValid);
            Assert.AreEqual(1, errorCount);
            Assert.AreEqual(ErrorSavingFileToDb, error);
        }

        [Test]
        public void CreateAsync_ErrorAddingFileToBlobStorage_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);
            SetupController();
            SetupPostedFile();
            SetupUploadBlobResult(false);
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            _fileService.Setup(x => x.FileValidation(It.IsAny<HttpPostedFileBase>())).Returns(_validateBlobResult);
            _fileService.Setup(x => x.Create(_fileWriteViewModel)).Returns(_fileId);
            _fileService.Setup(x => x.UploadFileAsync(_postedFile.Object, It.IsAny<string>(), CancellationToken.None)).Returns(Task.FromResult(_uploadBlobResult));
            _folderService.Setup(x => x.IsFolderIdValidAsync(_folderId, CancellationToken.None)).Returns(Task.FromResult(true));

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(StorageUploadError, response.Message);
        }

        [Test]
        public async Task CreateAsync_FileSavedToDbAndUploadedToBlobStorage_ReturnsRedirectToRouteResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);
            SetupController();
            SetupPostedFile();
            SetupUploadBlobResult(true);
            _fileUploadViewModel.FileToUpload = _fileWriteViewModel;

            _fileService.Setup(x => x.FileValidation(It.IsAny<HttpPostedFileBase>())).Returns(_validateBlobResult);
            _fileService.Setup(x => x.Create(_fileWriteViewModel)).Returns(_fileId);
            _fileService.Setup(x => x.UploadFileAsync(_postedFile.Object, It.IsAny<string>(), CancellationToken.None)).Returns(Task.FromResult(_uploadBlobResult));
            _folderService.Setup(x => x.IsFolderIdValidAsync(_folderId, CancellationToken.None)).Returns(Task.FromResult(true));

            var response = await _groupFileController.CreateAsync(_fileUploadViewModel, CancellationToken.None);
            var responseRouteName = ((RedirectToRouteResult)response).RouteName;

            Assert.IsInstanceOf<RedirectToRouteResult>(response);
            Assert.AreEqual(RouteName, responseRouteName);
        }

        // ===========================================================================================================================
        // UpdateAsync

        [Test]
        public void UpdateAsync_FileIdParameterEmpty_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFileController.UpdateAsync(Guid.Empty, null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FileIdParameterName, response.ParamName);
        }

        [Test]
        public void UpdateAsync_FileNotFoundForId_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.UpdateAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UpdateFileNotFoundForIdError, response.Message);
        }

        [Test]
        public void UpdateAsync_UserNotLoggedIn_ThrowsException()
        {
            SetupFileForRead(null);
            var response = Assert.ThrowsAsync<NullReferenceException>(async () => await _groupFileController.UpdateAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void UpdateAsync_UserNotFoundForId_ThrowsException()
        {
            SetupFileForRead(null);
            SetUserInContext(AddUpdateUserName);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.UpdateAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        public async Task UpdateAsync_UserDoesNotHaveFileUpdateAccess_ReturnsRedirectResult()
        {
            SetupFileForRead(null);
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFolderWriteAccess(false);

            var response = await _groupFileController.UpdateAsync(_fileId, null, CancellationToken.None) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public async Task UpdateAsync_HasFolderWriteAccessUpdateAllowed_ReturnsViewResult()
        {
            SetupFileForRead(null);
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFolderWriteAccess(true);
            SetupController();

            var response = await _groupFileController.UpdateAsync(_fileId, null, CancellationToken.None) as ViewResult;

            var responseModel = response.Model;
            var fileIdForUpdate = (response.Model as FileUpdateViewModel).FileId;

            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileUpdateViewModel>(responseModel);
            Assert.AreEqual(_fileId, fileIdForUpdate);
        }

        [Test]
        public async Task UpdateAsync_HasFileWriteAccessUpdateAllowed_ReturnsViewResult()
        {
            SetupFileForRead(null);
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFolderWriteAccess(false);
            SetupController();

            _fileReadViewModel.CreatedBy = _addUpdateUserId;

            var response = await _groupFileController.UpdateAsync(_fileId, null, CancellationToken.None) as ViewResult;

            var responseModel = response.Model;
            var fileIdForUpdate = (response.Model as FileUpdateViewModel).FileId;

            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileUpdateViewModel>(responseModel);
            Assert.AreEqual(_fileId, fileIdForUpdate);
        }

        // ===========================================================================================================================
        // UpdateAsync - post

        [Test]
        public void UpdateAsyncPost_FileIdParameterIsEmpty_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFileController.UpdateAsync(Guid.Empty, null, null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FileIdParameterName, response.ParamName);
        }

        [Test]
        public void UpdateAsyncPost_SlugParameterIsNull_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupFileController.UpdateAsync(_fileId, null, null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(SlugParameterName, response.ParamName);
        }

        [Test]
        public void UpdateAsyncPost_FileParameterIsNull_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupFileController.UpdateAsync(_fileId, _slug, null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileParameterName, response.ParamName);
        }

        [Test]
        public void UpdateAsyncPost_FileNotFoundForId_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.UpdateAsync(_fileId, _slug, _fileUpdateViewModel, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UpdateFileNotFoundForIdError, response.Message);
        }

        [Test]
        public void UpdateAsyncPost_UserNotLoggedIn_ThrowsException()
        {
            SetupFileForRead(null);
            SetupFileForUpdate();

            var response = Assert.ThrowsAsync<NullReferenceException>(async () => await _groupFileController.UpdateAsync(_fileId, _slug, _fileUpdateViewModel, CancellationToken.None));

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void UpdateAsyncPost_UserNotFoundForId_ThrowsException()
        {
            SetupFileForRead(null);
            SetupFileForUpdate();
            SetUserInContext(AddUpdateUserName);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.UpdateAsync(_fileId, _slug, _fileUpdateViewModel, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task UpdateAsyncPost_UserDoesNotHaveFileUdateAccess_ReturnsRedirectResult()
        {
            SetupFileForRead(null);
            SetupFileForUpdate();
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFolderWriteAccess(false);

            _fileReadViewModel.CreatedBy = Guid.NewGuid();

            var response = await _groupFileController.UpdateAsync(_fileId, _slug, _fileUpdateViewModel, CancellationToken.None) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public async Task UpdateAsyncPost_HasFolderWriteAccessUpdateAllowed_ReturnsViewResult()
        {
            SetupFileForRead(null);
            SetupFileForUpdate();
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFolderWriteAccess(true);

            _fileService.Setup(x => x.UpdateAsync(It.IsAny<FileUpdateViewModel>(), CancellationToken.None)).Returns(Task.FromResult(true));

            var response = await _groupFileController.UpdateAsync(_fileId, _slug, _fileUpdateViewModel, CancellationToken.None) as RedirectToRouteResult;

            var responseRouteName = ((RedirectToRouteResult)response).RouteName;

            Assert.IsInstanceOf<RedirectToRouteResult>(response);
            Assert.AreEqual(FilesRouteName, responseRouteName);
        }

        [Test]
        public async Task UpdateAsyncPost_HasFileWriteAccessUpdateAllowed_ReturnsViewResult()
        {
            SetupFileForRead(null);
            SetupFileForUpdate();
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFolderWriteAccess(false);

            _fileReadViewModel.CreatedBy = _addUpdateUserId;

            _fileService.Setup(x => x.UpdateAsync(It.IsAny<FileUpdateViewModel>(), CancellationToken.None)).Returns(Task.FromResult(true));

            var response = await _groupFileController.UpdateAsync(_fileId, _slug, _fileUpdateViewModel, CancellationToken.None) as RedirectToRouteResult;

            var responseRouteName = ((RedirectToRouteResult)response).RouteName;

            Assert.IsInstanceOf<RedirectToRouteResult>(response);
            Assert.AreEqual(FilesRouteName, responseRouteName);
        }

        // ===========================================================================================================================
        // DeleteAsync

        [Test]
        public void DeleteAsync_FileIdParameterEmpty_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFileController.DeleteAsync(Guid.Empty, null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FileIdParameterName, response.ParamName);
        }

        [Test]
        public void DeleteAsync_UserNotLoggedIn_ThrowsException()
        {
            SetupFileForRead(null);

            var response = Assert.ThrowsAsync<NullReferenceException>(async () => await _groupFileController.DeleteAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void DeleteAsync_UserNotFoundForId_ThrowsException()
        {
            SetupFileForRead(null);
            SetUserInContext(AddUpdateUserName);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.DeleteAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task DeleteAsync_UserDoesNotHaveGroupAccess_ReturnsRedirectResult()
        {
            SetupFileForRead(null);
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(false);

            var response = await _groupFileController.DeleteAsync(_fileId, null, CancellationToken.None) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public void DeleteAsync_FileNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.DeleteAsync(_fileId, null, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(DeleteFileNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task DeleteAsync_Success_ReturnsViewResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);
            SetupFileForRead(null);

            var response = await _groupFileController.DeleteAsync(_fileId, null, CancellationToken.None) as ViewResult;

            var responseModel = response.Model;
            var fileIdForUpdate = (response.Model as FileWriteViewModel).FileId;

            Assert.IsInstanceOf<ActionResult>(response);
            Assert.IsInstanceOf<FileWriteViewModel>(responseModel);
            Assert.AreEqual(_fileId, fileIdForUpdate);
        }

        // ===========================================================================================================================
        // Delete

        [Test]
        public void Delete_NoModelParameter_ThrowsException()
        {
            var response = Assert.Throws<ArgumentNullException>(delegate { _groupFileController.Delete(null); });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileParameterName, response.ParamName);
        }

        [Test]
        public void Delete_UserNotLoggedIn_ThrowsException()
        {
            var response = Assert.Throws<NullReferenceException>(delegate { _groupFileController.Delete(_fileWriteViewModel); });

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void Delete_UserNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);

            var response = Assert.Throws<ApplicationException>(delegate { _groupFileController.Delete(_fileWriteViewModel); });

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        [Test]
        public void Delete_UserDoesNotHaveGroupAccess_ReturnsRedirectResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(false);

            var response = _groupFileController.Delete(_fileWriteViewModel) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public void Delete_FileDeleted_ReturnsRedirectToRouteResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileWriteAccess(true);

            var response = _groupFileController.Delete(_fileWriteViewModel);

            var responseRouteName = ((RedirectToRouteResult)response).RouteName;

            Assert.IsInstanceOf<RedirectToRouteResult>(response);
            Assert.AreEqual(RouteName, responseRouteName);
        }

        // ===========================================================================================================================
        // DownloadAsync

        [Test]
        public void DownloadAsync_FileIdParameterEmpty_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFileController.DownloadAsync(Guid.Empty, string.Empty, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FileIdFullParameterName, response.ParamName);
        }

        [Test]
        public void DownloadAsync_UserNotLoggedIn_ThrowsException()
        {
            var response = Assert.ThrowsAsync<NullReferenceException>(async () => await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None));

            Assert.IsInstanceOf<NullReferenceException>(response);
            Assert.AreEqual(UserNotLoggedInError, response.Message);
        }

        [Test]
        public void DownloadAsync_UserNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(UserNotFoundForIdError, response.Message);
        }

        [Test]
        public async Task DownloadAsync_UserDoesNotHaveGroupAccess_ReturnsRedirectResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileAccess(false);

            var response = await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None) as RedirectToRouteResult;

            var routeValuesCount = response.RouteValues.Count;
            var routeValueAction = response.RouteValues[ActionKey];
            var routeValueController = response.RouteValues[ControllerKey];

            Assert.AreEqual(2, routeValuesCount);
            Assert.AreEqual(NoAccessRedirectAction, routeValueAction);
            Assert.AreEqual(NoAccessRedirectController, routeValueController);
        }

        [Test]
        public void DownloadAsync_FileNotFoundForId_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileAccess(true);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(DownloadFileNotFoundForIdError, response.Message);
        }

        [Test]
        public void DownloadAsync_FileHasInvalidName_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileAccess(true);
            SetupFileForRead(null);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(DownloadFileNoFileNameError, response.Message);
        }

        [Test]
        public void DownloadAsync_FileStatusNotVerified_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileAccess(true);
            SetupFileForRead(DownloadFileName);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(DownloadFileInvalidStatusError, response.Message);
        }

        [Test]
        public void DownloadAsync_DownloadPathNotSet_ThrowsException()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileAccess(true);
            SetupFileForRead(DownloadFileName, true);

            var response = Assert.ThrowsAsync<ApplicationException>(async () => await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None));

            Assert.IsInstanceOf<ApplicationException>(response);
            Assert.AreEqual(DownloadFileInvalidEndpointError, response.Message);
        }

        [Test]
        public async Task DownloadAsync_DownloadLinkGeneratedSuccessfully_ReturnsRedirectResult()
        {
            SetUserInContext(AddUpdateUserName);
            SetupLoggedInUser();
            SetupUserHasFileAccess(true);
            SetupFileForRead(DownloadFileName, true);

            _fileService.Setup(x => x.GetRelativeDownloadUrlAsync(DownloadFileName, Azure.Storage.Sas.BlobSasPermissions.Read, CancellationToken.None)).Returns(Task.FromResult(DownloadLink));

            var response = await _groupFileController.DownloadAsync(_fileId, string.Empty, CancellationToken.None); 

            var expectedDownloadPath = $"{DownloadPathPrefix}{DownloadLink}";
            var actualDownloadPath = (response as RedirectResult).Url;

            Assert.IsInstanceOf<RedirectResult>(response);
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);
        }

        // ===========================================================================================================================
        // Setup methods

        private void SetupFileForRead(string fileName, bool isVerified = false)
        {
            _fileReadViewModel.Id = _fileId;
            _fileReadViewModel.ParentFolder = _folderId;

            _fileService.Setup(x => x.GetFileAsync(_fileId, CancellationToken.None)).Returns(Task.FromResult(_fileReadViewModel));

            _fileReadViewModel.BlobName = string.IsNullOrWhiteSpace(fileName) ? null : fileName;
            _fileReadViewModel.Status = isVerified ? (int)Core.Models.Enums.UploadStatus.Verified : 0;
        }

        private void SetupFileForUpdate()
        {
            _fileUpdateViewModel.FileId = _fileId;

            //_fil
        }

        private void SetupController()
        {
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();

            var requestContext = new RequestContext(context.Object, new RouteData());
            var routes = new RouteCollection();

            routes.MapRoute(
                RouteName,            // Route name
                RouteUrl,               // URL with parameters
                new { controller = RouteController, action = RouteAction, slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            _groupFileController.Url = new System.Web.Mvc.UrlHelper(requestContext, routes);

            context.SetupGet(x => x.Request).Returns(request.Object); //Set up request base for httpcontext

            _groupFileController.ControllerContext = new ControllerContext(context.Object, new RouteData(), _groupFileController); //set controller context
        }

        private void SetContext()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
                );
        }

        private void SetUserInContext(string username)
        {
            SetContext();

            // User is logged in
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity(username),
                new string[0]
                );
        }

        private void SetupLoggedInUser()
        {
            _membershipService.Setup(x => x.GetUser(AddUpdateUserName, true)).Returns(new MembershipUser() { Id = _addUpdateUserId });
        }

        private void SetupUserHasFolderWriteAccess(bool hasAccess)
        {
            _folderService.Setup(x => x.UserHasFolderWriteAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(hasAccess));
        }

        private void SetupUserHasFileWriteAccess(bool hasAccess)
        {
            _folderService.Setup(x => x.UserHasFileWriteAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(hasAccess));
        }

        private void SetupUserHasFileAccess(bool hasAccess)
        {
            _fileService.Setup(x => x.UserHasFileAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(hasAccess));
        }

        private void SetupPostedFile()
        {
            _postedFile.Setup(x => x.FileName).Returns(PostedFileActualFileName);
            _postedFile.Setup(x => x.ContentLength).Returns(PostedFileContentLength);

            _fileWriteViewModel.PostedFile = _postedFile.Object;
        }

        private void SetupUploadBlobResult(bool successful)
        {
            _uploadBlobResult.UploadSuccessful = successful;

            if(successful)
            {
                _uploadBlobResult.UploadedFileName = FileUploadedFileName;
                _uploadBlobResult.UploadedFileHash = _fileUploadedFileHash;
            }
        }
    }
}
