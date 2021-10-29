using Moq;
using MvcForum.Core.Constants;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.Entities;
using MvcForum.Core.Models.FilesAndFolders;
using MvcForum.Core.Repositories.Models;
using MvcForum.Web.Controllers;
using MvcForum.Web.Tests.Controllers.Base;
using MvcForum.Web.ViewModels.Folder;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcForum.Web.Tests.Controllers.Folder
{
    public class FolderControllerTests
    {
        private readonly Mock<IFolderService> _folderService;

        private readonly Mock<IFeatureManager> _featureManager;

        private readonly Mock<IMembershipService> _membershipService;

        private readonly Mock<ILocalizationService> _localisationService;

        private readonly Mock<IGroupService> _groupService;

        private FolderController _controller;

        private FolderWriteViewModel _model;

        private const string FOLDER_NAME = "FolderName";

        private const string GROUP_SLUG = "example-group";

        private const string FOLDER_DESCRIPTION = "A folder description";

        public FolderControllerTests()
        {
            _folderService = FolderServiceMocks.GetFolderService();
            _featureManager = new Mock<IFeatureManager>();
            _localisationService = new Mock<ILocalizationService>();
            _groupService = new Mock<IGroupService>();
            _membershipService = new Mock<IMembershipService>();
        }

        [SetUp]
        public void SetUp()
        {
            _membershipService.Setup(
                m => m.GetUser(It.IsAny<string>(), It.IsAny<bool>())
                )
                .Returns(new MembershipUser());

            _featureManager.Setup(m =>
                m.IsEnabled(It.IsAny<string>())
            )
            .Returns(true);

            _controller = new FolderController(
                _folderService.Object,
                _featureManager.Object,
                _membershipService.Object,
                _localisationService.Object,
                _groupService.Object);

            _model = new FolderWriteViewModel()
            {
                FolderName = FOLDER_NAME,
                FolderId = Guid.NewGuid(),
                ParentFolder = Guid.Empty,
                Slug = GROUP_SLUG,
                Description = FOLDER_DESCRIPTION
            };

            SetUserInContext.SetContext("admin");
        }

        [Test]
        public async Task Handles_ValidGetRequest_DeleteFolderAsync()
        {
            var result = await _controller.DeleteFolderAsync(_model, CancellationToken.None);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(_model.Slug, (result as RedirectToRouteResult).RouteValues["slug"]);
            Assert.AreEqual(Constants.GroupFilesTab, (result as RedirectToRouteResult).RouteValues["tab"]);
        }

        [Test]
        public async Task Handles_ErrorProcessingDelete_DeleteFolderAsync()
        {
            _folderService.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _controller.DeleteFolderAsync(_model, CancellationToken.None);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(_model.Slug, (result as RedirectToRouteResult).RouteValues["slug"]);
            Assert.AreEqual(Constants.GroupFilesTab, (result as RedirectToRouteResult).RouteValues["tab"]);
            Assert.IsTrue((bool)(result as RedirectToRouteResult).RouteValues["HasError"]);
        }

        [Test]
        public async Task Handles_UnauthenticatedUser_DeleteFolderAsync()
        {
            _folderService.Setup(m => m.UserIsAdmin(It.IsAny<string>(), It.IsAny<Guid>())).Returns(false);

            var result = await _controller.DeleteFolderAsync(_model, CancellationToken.None);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(_model.Slug, (result as RedirectToRouteResult).RouteValues["slug"]);
            Assert.AreEqual(Constants.GroupFilesTab, (result as RedirectToRouteResult).RouteValues["tab"]);
            Assert.IsTrue((bool)(result as RedirectToRouteResult).RouteValues["HasError"]);
        }

        [Test]
        public async Task Handles_FolderNotFound_DeleteFolderAsync()
        {
            _folderService.Setup(m => m.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FolderViewModel { Folder = new FolderReadViewModel { FolderId = Guid.Empty } });

            var result = await _controller.DeleteFolderAsync(_model, CancellationToken.None);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(_model.Slug, (result as RedirectToRouteResult).RouteValues["slug"]);
            Assert.AreEqual(Constants.GroupFilesTab, (result as RedirectToRouteResult).RouteValues["tab"]);
            Assert.IsTrue((bool)(result as RedirectToRouteResult).RouteValues["HasError"]);
        }
    }
}
