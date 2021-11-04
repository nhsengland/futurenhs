using Moq;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Repositories.Models;
using MvcForum.Web.ViewModels.Folder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Web.Tests.Controllers.Folder
{
    public static class FolderServiceMocks
    {

        private const string FOLDER_NAME = "FolderName";

        private const string GROUP_SLUG = "example-group";

        private const string FOLDER_DESCRIPTION = "A folder description";

        public static Mock<IFolderService> GetFolderService()
        {
            var folderServiceMock = new Mock<IFolderService>();

            var folderViewModel = new FolderViewModel
            {
                Folder = new FolderReadViewModel
                {
                    FolderName = FOLDER_NAME,
                    FolderId = Guid.NewGuid(),
                    ParentId = Guid.Empty,
                    Slug = GROUP_SLUG,
                    Description = FOLDER_DESCRIPTION
                }
            };

            folderServiceMock.Setup(m =>
                m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            ).ReturnsAsync(true);

            folderServiceMock.Setup(
                m => m.UserIsAdmin(It.IsAny<string>(), It.IsAny<Guid>()
                )).Returns(true);


            folderServiceMock.Setup(
                m => m.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())
                ).ReturnsAsync(folderViewModel);

            return folderServiceMock;
        }

        public static Mock<IFolderService> GetFailingFolderService()
        {
            var folderServiceMock = new Mock<IFolderService>();

            var folderViewModel = new FolderViewModel
            {
                Folder = new FolderReadViewModel
                {
                    FolderName = FOLDER_NAME,
                    FolderId = Guid.NewGuid(),
                    ParentId = Guid.Empty,
                    Slug = GROUP_SLUG,
                    Description = FOLDER_DESCRIPTION
                }
            };

            folderServiceMock.Setup(m =>
                m.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            ).ReturnsAsync(false);

            folderServiceMock.Setup(
                m => m.UserIsAdmin(It.IsAny<string>(), It.IsAny<Guid>()
                )).Returns(true);


            folderServiceMock.Setup(
                m => m.GetFolderAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())
                ).ReturnsAsync(folderViewModel);

            return folderServiceMock;
        }
    }
}
