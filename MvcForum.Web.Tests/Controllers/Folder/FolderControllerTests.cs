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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcForum.Web.Tests.Controllers.Folder
{
    internal class FolderControllerTests
    {
        private readonly Mock<IFolderService> _folderServiceMocks;
        private readonly Mock<IFeatureManager> _featureManagerMocks;
        private readonly Mock<IMembershipService> _membershipServiceMocks;
        private readonly Mock<ILocalizationService> _localizationServiceMocks;
        private readonly Mock<IGroupService> _groupServiceMocks;

        private static readonly Guid GroupIdValid = Guid.NewGuid();
        private static readonly Guid GroupIdInvalid = Guid.Empty;
        private static readonly Guid FolderIdValid = Guid.NewGuid();
        private const string GroupSlugValid = "group-slug";
        private static readonly string GroupSlugInvalid = string.Empty;
        private const string FolderNameValid = "validName";
        private const string FolderDescription = "validDescription";
        private const string DuplicateFolderError = "Folder.Error.DuplicateTitle";

        private FolderController _folderController;

        private FolderWriteViewModel _model;

        public FolderControllerTests()
        {
            _folderServiceMocks = new Mock<IFolderService>();
            _featureManagerMocks = new Mock<IFeatureManager>();
            _membershipServiceMocks = new Mock<IMembershipService>();
            _localizationServiceMocks = new Mock<ILocalizationService>();
            _groupServiceMocks = new Mock<IGroupService>();
        }

        [SetUp]
        public void SetUp()
        {
            _folderController = new FolderController(_folderServiceMocks.Object, _featureManagerMocks.Object, _membershipServiceMocks.Object, _localizationServiceMocks.Object, _groupServiceMocks.Object);

            _model = new FolderWriteViewModel()
            {
                FolderName = FolderNameValid,
                FolderId = Guid.NewGuid(),
                ParentFolder = Guid.Empty,
                Slug = GroupSlugValid,
                Description = FolderDescription
            };

            _featureManagerMocks.Setup(x => x.IsEnabled(It.IsAny<string>()))
                .Returns(true);

            _membershipServiceMocks.Setup(x => x.GetUser(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new MembershipUser()
                {
                    Id = Guid.NewGuid()
                });

            _localizationServiceMocks.Setup(x => x.GetResourceString(DuplicateFolderError))
                .Returns(DuplicateFolderError);

            _folderServiceMocks.Setup(service => service.GenerateBreadcrumbTrailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid folderId, CancellationToken cancellationToken) =>
                {
                    return new List<BreadCrumbItem>()
                    {
                        new BreadCrumbItem()
                        {
                            Id = FolderIdValid,
                            Name = "Breadcrumb Name"
                        }
                    };
                });

        }

        [Test]
        public async Task CreateFolderAsync_ValidGetRequest_Handle()
        {
            SetUserInContext.SetContext("admin");
            _folderServiceMocks.Setup(x => x.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var result = (await _folderController.CreateFolderAsync(GroupSlugValid, null, GroupIdValid, CancellationToken.None)) as ViewResult;
            var responseViewModelSlug = (result.Model as FolderWriteViewModel).Slug;
            var responseViewModelParentGroupId = (result.Model as FolderWriteViewModel).ParentGroup;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<FolderWriteViewModel>(result.Model);
            Assert.AreEqual(GroupSlugValid, responseViewModelSlug);
            Assert.AreEqual(GroupIdValid, responseViewModelParentGroupId);
        }

        [Test]
        public async Task CreateFolderAsync_InvalidUserGetRequest_Handle()
        {
            SetUserInContext.SetContext("user");
            _folderServiceMocks.Setup(x => x.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var result = await _folderController.CreateFolderAsync(GroupSlugValid, null, GroupIdValid, CancellationToken.None);
            var redirectedRouteName = (result as RedirectToRouteResult).RouteName;
            var redirectedFolderId = (result as RedirectToRouteResult).RouteValues["folder"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("GroupUrls", redirectedRouteName);
            Assert.AreNotEqual(Guid.Empty, redirectedFolderId);
        }

        [Test]
        public async Task CreateFolderAsync_ValidPostRequest_Handle()
        {
            var model = new FolderWriteViewModel()
            {
                Slug = GroupSlugValid,
                ParentGroup = GroupIdValid,
                ParentFolder = null,
                FolderName = FolderNameValid,
                Description = FolderDescription
            };
            SetUserInContext.SetContext("admin");
            _folderServiceMocks.Setup(x => x.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            _folderServiceMocks.Setup(x => x.IsFolderNameValidAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            _folderServiceMocks.Setup(x => x.CreateFolder(It.IsAny<FolderWriteViewModel>()))
                .Returns(Guid.NewGuid());

            var result = await _folderController.CreateFolderAsync(model, CancellationToken.None);
            var redirectedRouteName = (result as RedirectToRouteResult).RouteName;
            var redirectedFolderId = (result as RedirectToRouteResult).RouteValues["folder"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("GroupUrls", redirectedRouteName);
            Assert.AreNotEqual(Guid.Empty, redirectedFolderId);
        }

        [Test]
        public async Task CreateFolderAsync_InvalidModelPostRequest_Handle()
        {
            var model = new FolderWriteViewModel()
            {
                Slug = GroupSlugInvalid,
                ParentGroup = GroupIdInvalid,
                ParentFolder = null,
                FolderName = null,
                FolderId = FolderIdValid,
                Description = null
            };
            SetUserInContext.SetContext("admin");
            SetRouting<FolderController>.SetupController(_folderController, "GroupUrls", "routeUrl", "routeController", "routeAction");
            _folderController.ModelState.AddModelError("error", "invalid state");
            _folderServiceMocks.Setup(x => x.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var result = (await _folderController.CreateFolderAsync(model, CancellationToken.None)) as ViewResult;
            var returnedViewModelSlug = (result.Model as FolderWriteViewModel).Slug;
            var returnedViewModelParentGroupId = (result.Model as FolderWriteViewModel).ParentGroup;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<FolderWriteViewModel>(result.Model);
            Assert.AreEqual(GroupSlugInvalid, returnedViewModelSlug);
            Assert.AreEqual(GroupIdInvalid, returnedViewModelParentGroupId);
        }

        [Test]
        public async Task CreateFolderAsync_InvalidFolderNamePostRequest_Handle()
        {
            var model = new FolderWriteViewModel()
            {
                Slug = GroupSlugValid,
                ParentGroup = GroupIdInvalid,
                ParentFolder = null,
                FolderName = null,
                Description = null
            };
            SetUserInContext.SetContext("admin");
            _folderServiceMocks.Setup(x => x.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            _folderServiceMocks.Setup(x => x.IsFolderNameValidAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var result = (await _folderController.CreateFolderAsync(model, CancellationToken.None)) as ViewResult;
            var returnedErrorMessage = result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<FolderWriteViewModel>(result.Model);
            Assert.AreEqual(DuplicateFolderError, returnedErrorMessage);
        }

        [Test]
        public async Task UpdateFolderAsync_ValidGetRequest_Handle()
        {
            SetUserInContext.SetContext("admin");
            SetRouting<FolderController>.SetupController(_folderController, "GroupUrls", "routeUrl", "routeController", "routeAction");
            _folderServiceMocks.Setup(x => x.UserHasFolderWriteAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
            _folderServiceMocks.Setup(x => x.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new FolderViewModel()
                {
                    Folder = new FolderReadViewModel(),
                    Slug = GroupSlugValid
                }));

            var result = (await _folderController.UpdateFolderAsync(GroupSlugValid, GroupIdValid, FolderIdValid, null, CancellationToken.None)) as ViewResult;
            var returnedViewModelSlug = (result.Model as FolderWriteViewModel).Slug;
            var returnedViewModelParentGroupId = (result.Model as FolderWriteViewModel).ParentGroup;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<FolderWriteViewModel>(result.Model);
            Assert.AreEqual(GroupSlugValid, returnedViewModelSlug);
            Assert.AreEqual(GroupIdValid, returnedViewModelParentGroupId);
        }

        [Test]
        public async Task UpdateFolderAsync_InvalidUserGetRequest_Handle()
        {
            SetUserInContext.SetContext("user");
            SetRouting<FolderController>.SetupController(_folderController, "GroupUrls", "routeUrl", "routeController", "routeAction");
            _folderServiceMocks.Setup(x => x.UserHasFolderWriteAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));
            _folderServiceMocks.Setup(x => x.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new FolderViewModel()
                {
                    Folder = new FolderReadViewModel(),
                    Slug = GroupSlugValid
                }));

            var result = await _folderController.UpdateFolderAsync(GroupSlugValid, GroupIdValid, FolderIdValid, null, CancellationToken.None);
            var redirectedRouteName = (result as RedirectToRouteResult).RouteName;
            var redirectedFolderId = (result as RedirectToRouteResult).RouteValues["folder"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("GroupUrls", redirectedRouteName);
            Assert.AreNotEqual(Guid.Empty, redirectedFolderId);
        }

        [Test]
        public async Task UpdateFolderAsync_ValidPostRequest_Handle()
        {
            var model = new FolderWriteViewModel()
            {
                Slug = GroupSlugValid,
                FolderId = FolderIdValid,
                FolderName = FolderNameValid,
                Description = FolderDescription,
                ParentGroup = GroupIdValid,
                ParentFolder = null,
            };
            SetUserInContext.SetContext("admin");
            SetRouting<FolderController>.SetupController(_folderController, "GroupUrls", "routeUrl", "routeController", "routeAction");
            _folderServiceMocks.Setup(x => x.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            _folderServiceMocks.Setup(x => x.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new FolderViewModel()
                {
                    Folder = new FolderReadViewModel()
                    {
                        FolderId = FolderIdValid
                    }
                }));
            _folderServiceMocks.Setup(x => x.IsFolderNameValidAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var result = await _folderController.UpdateFolderAsync(model, CancellationToken.None);
            var redirectedRouteName = (result as RedirectToRouteResult).RouteName;
            var redirectedFolderId = (result as RedirectToRouteResult).RouteValues["folder"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("GroupUrls", redirectedRouteName);
            Assert.AreNotEqual(Guid.Empty, redirectedFolderId);
        }

        [Test]
        public async Task UpdateFolderAsync_InvalidModelPostRequest_Handle()
        {
            var model = new FolderWriteViewModel()
            {
                Slug = GroupSlugInvalid,
                FolderName = FolderNameValid,
                Description = FolderDescription,
                ParentGroup = GroupIdValid,
                ParentFolder = null,
            };
            _folderController.ModelState.AddModelError("error", "invalid state");
            SetUserInContext.SetContext("admin");
            SetRouting<FolderController>.SetupController(_folderController, "GroupUrls", "routeUrl", "routeController", "routeAction");
            _folderServiceMocks.Setup(x => x.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var result = (await _folderController.UpdateFolderAsync(model, CancellationToken.None)) as ViewResult;
            var returnedViewModelSlug = (result.Model as FolderWriteViewModel).Slug;
            var returnedViewModelParentGroupId = (result.Model as FolderWriteViewModel).ParentGroup;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<FolderWriteViewModel>(result.Model);
            Assert.AreEqual(GroupSlugInvalid, returnedViewModelSlug);
            Assert.AreEqual(GroupIdValid, returnedViewModelParentGroupId);
        }

        [Test]
        public async Task UpdateFolderAsync_InvalidFolderNamePostRequest_Handle()
        {
            var model = new FolderWriteViewModel()
            {
                Slug = GroupSlugValid,
                FolderId = FolderIdValid,
                FolderName = null,
                ParentGroup = GroupIdInvalid,
                ParentFolder = null,
                Description = null
            };
            SetUserInContext.SetContext("admin");
            SetRouting<FolderController>.SetupController(_folderController, "GroupUrls", "routeUrl", "routeController", "routeAction");
            _folderServiceMocks.Setup(x => x.UserHasFolderWriteAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            _folderServiceMocks.Setup(x => x.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new FolderViewModel()
                {
                    Folder = new FolderReadViewModel()
                    {
                        FolderId = FolderIdValid
                    }
                }));
            _folderServiceMocks.Setup(x => x.IsFolderNameValidAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var result = (await _folderController.UpdateFolderAsync(model, CancellationToken.None)) as ViewResult;
            var returnedErrorMessage = result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<FolderWriteViewModel>(result.Model);
            Assert.AreEqual(DuplicateFolderError, returnedErrorMessage);
        }

        [Test]
        public void GetFolderAsync_ValidGetRequest_Handle()
        {
            SetUserInContext.SetContext("admin");
            _folderServiceMocks.Setup(x => x.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new FolderViewModel()
                {
                    Folder = new FolderReadViewModel()
                    {
                        FolderId = FolderIdValid,
                        Slug = GroupSlugValid                        
                    }
                }));
            _folderServiceMocks.Setup(x => x.UserHasFolderReadAccessAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
            _groupServiceMocks.Setup(service => service.GetAllForUser(It.IsAny<Guid>())).Returns(
                (Guid userId) =>
                {
                    return new List<GroupUser>();
                });

            var result = _folderController.GetFolder(GroupSlugValid, null, GroupIdValid);
            var returnedViewModelFolderSlug = (result.Model as FolderViewModel).Folder.Slug;
            var returnedViewModelGroupId = (result.Model as FolderViewModel).GroupId;

            Assert.IsInstanceOf<PartialViewResult>(result);
            Assert.AreEqual(GroupSlugValid, returnedViewModelFolderSlug);
            Assert.AreEqual(GroupIdValid, returnedViewModelGroupId);
        }

        [Test]
        public async Task Handles_ValidGetRequest_DeleteFolderAsync()
        {
            SetUserInContext.SetContext("admin");

            var result = await _folderController.DeleteFolderAsync(_model, CancellationToken.None);
            var redirectedRouteSlug = (result as RedirectToRouteResult).RouteValues["slug"];
            var redirectedRouteTab = (result as RedirectToRouteResult).RouteValues["tab"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(GroupSlugValid, redirectedRouteSlug);
            Assert.AreEqual(Constants.GroupFilesTab, redirectedRouteTab);
        }

        [Test]
        public async Task Handles_ErrorProcessingDelete_DeleteFolderAsync()
        {
            SetUserInContext.SetContext("admin");
            _folderServiceMocks.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _folderController.DeleteFolderAsync(_model, CancellationToken.None);
            var redirectedRouteSlug = (result as RedirectToRouteResult).RouteValues["slug"];
            var redirectedRouteTab = (result as RedirectToRouteResult).RouteValues["tab"];
            var redirectedRouteError = (bool)(result as RedirectToRouteResult).RouteValues["HasError"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(GroupSlugValid, redirectedRouteSlug);
            Assert.AreEqual(Constants.GroupFilesTab, redirectedRouteTab);
            Assert.IsTrue(redirectedRouteError);
        }

        [Test]
        public async Task Handles_UnauthenticatedUser_DeleteFolderAsync()
        {
            SetUserInContext.SetContext("admin");
            _folderServiceMocks.Setup(m => m.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var result = await _folderController.DeleteFolderAsync(_model, CancellationToken.None);
            var redirectedRouteSlug = (result as RedirectToRouteResult).RouteValues["slug"];
            var redirectedRouteTab = (result as RedirectToRouteResult).RouteValues["tab"];
            var redirectedRouteError = (bool)(result as RedirectToRouteResult).RouteValues["HasError"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(GroupSlugValid, redirectedRouteSlug);
            Assert.AreEqual(Constants.GroupFilesTab, redirectedRouteTab);
            Assert.IsTrue(redirectedRouteError);
        }

        [Test]
        public async Task Handles_FolderNotFound_DeleteFolderAsync()
        {
            SetUserInContext.SetContext("admin");
            _folderServiceMocks.Setup(m => m.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FolderViewModel { Folder = new FolderReadViewModel { FolderId = Guid.Empty } });

            var result = await _folderController.DeleteFolderAsync(_model, CancellationToken.None);
            var redirectedRouteSlug = (result as RedirectToRouteResult).RouteValues["slug"];
            var redirectedRouteTab = (result as RedirectToRouteResult).RouteValues["tab"];
            var redirectedRouteError = (bool)(result as RedirectToRouteResult).RouteValues["HasError"];

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(GroupSlugValid, redirectedRouteSlug);
            Assert.AreEqual(Constants.GroupFilesTab, redirectedRouteTab);
            Assert.IsTrue(redirectedRouteError);
        }
    }
}
