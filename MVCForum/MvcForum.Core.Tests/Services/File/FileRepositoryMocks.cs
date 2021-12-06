using Moq;
using MvcForum.Core.Repositories.Repository.Interfaces;

namespace MvcForum.Core.Tests.Services.File
{
    internal class FileRepositoryMocks
    {
        internal static Mock<IFileRepository> GetFileService()
        {
            var mockFileRepository = new Mock<IFileRepository>();



            return mockFileRepository;
        }
    }
}
