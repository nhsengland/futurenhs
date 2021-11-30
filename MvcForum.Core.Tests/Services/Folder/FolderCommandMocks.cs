using Moq;
using MvcForum.Core.Repositories.Command.Interfaces;
using System;
using System.Threading;

namespace MvcForum.Core.Tests.Services.Folder
{
    internal class FolderCommandMocks
    {
        internal static Mock<IFolderCommand> GetFolderCommand()
        {
            var mockFolderCommand = new Mock<IFolderCommand>();

            mockFolderCommand.Setup(repo => repo.DeleteFolderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid folderId, CancellationToken cancellationToken) =>
                {
                    return true;
                });

            return mockFolderCommand;
        }
    }
}
