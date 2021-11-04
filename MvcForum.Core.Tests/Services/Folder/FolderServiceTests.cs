using Moq;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Repository.Interfaces;
using MvcForum.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Tests.Services.Folder
{
    public class FolderServiceTests
    {
        private readonly Mock<IFolderRepository> _folderRepository;

        private readonly Mock<IFileRepository> _fileRepository;

        private readonly Mock<IFolderCommand> _folderCommand;

        private readonly Mock<IFeatureManager> _featureManager;

        public FolderServiceTests()
        {
            _fileRepository = new Mock<IFileRepository>();
            _folderCommand = new Mock<IFolderCommand>();
            _featureManager = new Mock<IFeatureManager>();
            _folderRepository = new Mock<IFolderRepository>();
        }


        [Test]
        public async Task Handles_ValidDelete_DeleteFolderAsync()
        {
            _featureManager.Setup(m => m.IsEnabled(It.IsAny<string>())).Returns(true);
            _folderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var folderService = new FolderService(_folderRepository.Object, _featureManager.Object, _folderCommand.Object, _fileRepository.Object);

            var result = await folderService.DeleteFolderAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handles_InvalidDelete_DeleteFolderAsync()
        {
            _featureManager.Setup(m => m.IsEnabled(It.IsAny<string>())).Returns(true);
            _folderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var folderService = new FolderService(_folderRepository.Object, _featureManager.Object, _folderCommand.Object, _fileRepository.Object);

            var result = await folderService.DeleteFolderAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Handles_SqlException_DeleteFolderAsync()
        {
            _featureManager.Setup(m => m.IsEnabled(It.IsAny<string>())).Returns(true);
            _folderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(SqlExceptionMocks.NewSqlException(1));

            var folderService = new FolderService(_folderRepository.Object, _featureManager.Object, _folderCommand.Object, _fileRepository.Object);

            var result = await folderService.DeleteFolderAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_NullFolderId_DeleteFolderAsync()
        {
            _featureManager.Setup(m => m.IsEnabled(It.IsAny<string>())).Returns(true);
            _folderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false); _folderCommand.Setup(m => m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var folderService = new FolderService(_folderRepository.Object, _featureManager.Object, _folderCommand.Object, _fileRepository.Object);

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await folderService.DeleteFolderAsync(Guid.Empty, CancellationToken.None));
        }
    }
}
