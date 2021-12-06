using Moq;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Repository.Interfaces;
using MvcForum.Core.Services;
using MvcForum.Core.Tests.Services.Base;
using MvcForum.Core.Tests.Services.File;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Tests.Services.Folder
{
    internal class FolderServiceTests
    {
        private readonly Mock<IFolderRepository> _mockFolderRepository;
        private readonly Mock<IFolderCommand> _mockFolderCommand;
        private readonly Mock<IFeatureManager> _mockFeatureManager;
        private readonly Mock<IFileRepository> _mockFileRepository;

        private IFolderService _groupFolderService;

        private const string ValidSlug = "valid-slug";
        private const string ValidFolderName = "folderName";
        private static readonly Guid ValidFolderId = Guid.NewGuid();
        private static readonly Guid ValidGroupId = Guid.NewGuid();
        private static readonly Guid ValidAdminUserId = Guid.NewGuid();
        private static readonly Guid ValidNonAdminUserId = Guid.NewGuid();


        private readonly string InvalidSlug = string.Empty;
        private readonly string InvalidFolderName = string.Empty;
        private readonly Guid InvalidGuid = Guid.Empty;
        private static readonly Guid InvalidFolderId = Guid.NewGuid();


        public FolderServiceTests()
        {
            _mockFolderRepository = FolderRepositoryMocks.GetFolderRepository();
            _mockFolderCommand = FolderCommandMocks.GetFolderCommand();
            _mockFeatureManager = FeatureManagerMocks.GetFeatureManager();
            _mockFileRepository = FileRepositoryMocks.GetFileService();
        }

        [SetUp]
        public void SetUp()
        {
            _groupFolderService = new FolderService(_mockFolderRepository.Object, _mockFeatureManager.Object, _mockFolderCommand.Object, _mockFileRepository.Object);
        }

        [Test]
        public async Task GetFolderAsync_NullFolderId_Handle()
        {
            var response = await _groupFolderService.GetFolderAsync(ValidSlug, null, CancellationToken.None);
            var responseFolderSlug = response.ChildFolders.FirstOrDefault().Slug;

            Assert.AreEqual(ValidSlug, response.Slug);
            Assert.AreEqual(null, response.Folder);
            Assert.AreEqual(ValidSlug, responseFolderSlug);
        }

        [Test]
        public async Task GetFolderAsync_ValidFolderId_Handle()
        {
            var response = await _groupFolderService.GetFolderAsync(ValidSlug, ValidFolderId, CancellationToken.None);
            var responseFolderId = response.Folder.FolderId;

            Assert.AreEqual(ValidSlug, response.Slug);
            Assert.AreEqual(ValidFolderId, responseFolderId);
        }

        [Test]
        public void GetFolderAsync_InvalidSlug_Exception()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupFolderService.GetFolderAsync(InvalidSlug, ValidFolderId, CancellationToken.None));
            Assert.AreEqual("groupSlug", response.ParamName);
        }

        [Test]
        public async Task IsFolderNameValidAsync_ValidInput_Handle()
        {
            var response = await _groupFolderService.IsFolderNameValidAsync(ValidFolderName, null, ValidGroupId, CancellationToken.None);

            Assert.IsTrue(response);
        }

        [Test]
        public void IsFolderNameValidAsync_InvalidFolderName_Exception()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupFolderService.IsFolderNameValidAsync(InvalidFolderName, null, ValidGroupId, CancellationToken.None));
            Assert.AreEqual("folderName", response.ParamName);
        }

        [Test]
        public async Task IsFolderIdValidAsync_ValidInput_Handle()
        {
            var response = await _groupFolderService.IsFolderIdValidAsync(ValidFolderId, CancellationToken.None);

            Assert.IsTrue(response);
        }

        [Test]
        public void IsFolderIdAsync_InvalidFolderId_Exception()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFolderService.IsFolderIdValidAsync(InvalidGuid, CancellationToken.None));
            Assert.AreEqual("folderId", response.ParamName);
        }

        [Test]
        public void IsUserAdminAsync_InvalidParentGroupId_Exception()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFolderService.IsFolderNameValidAsync(ValidFolderName, null, InvalidGuid, CancellationToken.None));
            Assert.AreEqual("parentGroupId", response.ParamName);
        }

        [Test]
        public async Task IsUserAdminAsync_ValidAdminUserId_Handle()
        {
            _mockFolderRepository.Setup(repo => repo.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var response = await _groupFolderService.IsUserAdminAsync(ValidSlug, ValidAdminUserId, CancellationToken.None);

            Assert.IsTrue(response);
        }

        [Test]
        public async Task IsUserAdminAsync_ValidNonAdminUserId_Handle()
        {
            _mockFolderRepository.Setup(repo => repo.IsUserAdminAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var response = await _groupFolderService.IsUserAdminAsync(ValidSlug, ValidNonAdminUserId, CancellationToken.None);

            Assert.IsFalse(response);
        }

        [Test]
        public void IsUserAdminAsync_InvalidGroupSlug_Exception()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupFolderService.IsUserAdminAsync(null, ValidAdminUserId, CancellationToken.None));
            Assert.AreEqual("groupSlug", response.ParamName);
        }

        [Test]
        public void IsUserAdminAsync_InvalidUserId_Exception()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFolderService.IsUserAdminAsync(ValidSlug, InvalidGuid, CancellationToken.None));
            Assert.AreEqual("userId", response.ParamName);
        }

        [Test]
        public async Task UserHasGroupAccessAsync_ValidInput_Handle()
        {
            var response = await _groupFolderService.UserHasGroupAccessAsync(ValidSlug, ValidNonAdminUserId, CancellationToken.None);

            Assert.IsTrue(response);
        }

        [Test]
        public async Task UserHasGroupAccessAsync_InvalidGroupSlug_Handle()
        {
            var response = await _groupFolderService.UserHasGroupAccessAsync(InvalidSlug, ValidNonAdminUserId, CancellationToken.None);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task UserHasGroupAccessAsync_InvalidUserId_Handle()
        {
            var response = await _groupFolderService.UserHasGroupAccessAsync(ValidSlug, InvalidGuid, CancellationToken.None);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task GenerateBreadcrumbTrailAsync_ValidInput_Handle()
        {
            var response = await _groupFolderService.GenerateBreadcrumbTrailAsync(ValidFolderId, CancellationToken.None);
            var responseFolderId = response.FirstOrDefault().Id;

            Assert.AreEqual(ValidFolderId, responseFolderId);
        }

        [Test]
        public void GenerateBreadcrumbTrailAsync_InvalidFolderId_Exception()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFolderService.GenerateBreadcrumbTrailAsync(InvalidGuid, CancellationToken.None));
            Assert.AreEqual("folderId", response.ParamName);
        }

        [Test]
        public async Task Handles_ValidDelete_DeleteFolderAsync()
        {
            _mockFolderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _groupFolderService.DeleteFolderAsync(ValidFolderId, CancellationToken.None);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handles_InvalidDelete_DeleteFolderAsync()
        {
            _mockFolderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _groupFolderService.DeleteFolderAsync(InvalidFolderId, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Handles_SqlException_DeleteFolderAsync()
        {
            _mockFolderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false); 

            var result = await _groupFolderService.DeleteFolderAsync(InvalidFolderId, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_NullFolderId_DeleteFolderAsync()
        {
            var response = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupFolderService.DeleteFolderAsync(Guid.Empty, CancellationToken.None));
            Assert.AreEqual("folderId", response.ParamName);
        }
    }
}
