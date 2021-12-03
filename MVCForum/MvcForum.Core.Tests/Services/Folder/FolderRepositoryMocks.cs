using Moq;
using MvcForum.Core.Models.General;
using MvcForum.Core.Repositories.Models;
using MvcForum.Core.Repositories.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MvcForum.Core.Tests.Services.Folder
{
    public class FolderRepositoryMocks
    {
        public static Mock<IFolderRepository> GetFolderRepository()
        {
            var mockFolderRepository = new Mock<IFolderRepository>();

            mockFolderRepository.Setup(repo => repo.GetFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid folderId, CancellationToken cancellationToken) =>
                {
                    return new FolderReadViewModel()
                    {
                        FolderId = folderId
                    };
                });

            mockFolderRepository.Setup(repo => repo.GetRootFoldersForGroupAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (string groupSlug, int page, int pageSize, CancellationToken cancellationToken) =>
                {
                    var folderList = new List<FolderReadViewModel>(){
                        new FolderReadViewModel()
                        {
                            Slug = groupSlug                         
                        }
                    };

                    return new PaginatedList<FolderReadViewModel>(folderList, 20, page, pageSize);
                });

            mockFolderRepository.Setup(repo => repo.GetChildFoldersForFolderAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid parentFolderId, int page, int pageSize, CancellationToken cancellationToken) =>
                {
                    var folderList = new List<FolderReadViewModel>(){
                        new FolderReadViewModel()
                        {
                            ParentId = parentFolderId
                        }
                    };

                    return new PaginatedList<FolderReadViewModel>(folderList, 20, page, pageSize);
                });

            mockFolderRepository.Setup(repo => repo.IsFolderNameValidAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid? folderId, string folderName, Guid? parentFolderId, Guid parentGroupId, CancellationToken cancellationToken) =>
                {
                    if (string.IsNullOrEmpty(folderName))
                    {
                        return false;
                    }
                    return true;
                });

            mockFolderRepository.Setup(repo => repo.IsFolderIdValidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid folderId, CancellationToken cancellationToken) =>
                {
                    if (Guid.Empty == folderId)
                    {
                        return false;
                    }
                    return true;
                });

            mockFolderRepository.Setup(repo => repo.UserHasGroupAccessAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (string groupSlug, Guid userId, CancellationToken cancellationToken) =>
                {
                    return true;
                });

            mockFolderRepository.Setup(repo => repo.GenerateBreadcrumbTrailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid folderId, CancellationToken cancellationToken) =>
                {
                    return new List<BreadCrumbItem>()
                    {
                        new BreadCrumbItem()
                        {
                            Id = folderId,
                            Name = "Breadcrumb Item"
                        }
                    };
                });

            return mockFolderRepository;
        }
    }
}
