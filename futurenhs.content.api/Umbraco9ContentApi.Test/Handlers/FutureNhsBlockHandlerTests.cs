namespace Umbraco9ContentApi.Test.Handler
{
    using Core.Handlers.FutureNhs;
    using Core.Services.FutureNhs.Interface;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco9ContentApi.Core.Models.Content;
    using Assert = Xunit.Assert;

    [TestFixture]
    public class FutureNhsBlockHandlerTests
    {
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService;
        private Mock<IFutureNhsBlockService> _mockFutureNhsBlockService;
        private IConfiguration? _config;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentService = new Mock<IFutureNhsContentService>();
            _mockFutureNhsBlockService = new Mock<IFutureNhsBlockService>();
        }

        #region Get All Blocks Tests

        /// <summary>
        /// Gets all blocks success asynchronous.
        /// </summary>
        [Test]
        public async Task GetAllBlocks_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var mockContent = GetMockPublishedContentItem(true);
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var contentHandler = GetHandler(_config);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedChildrenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<IPublishedContent>()
                {
                    mockContent.Object
                });

            _mockFutureNhsContentService.SetupSequence(x => x.ResolveAsync(It.IsAny<IPublishedContent>()).Result)
                .Returns(new ContentModel() { Item = new ContentModelItem() { Id = contentId } });

            // Act
            var contentResult = await contentHandler.GetAllBlocksAsync();

            // Assert
            Assert.NotNull(contentResult);
            Assert.NotEmpty(contentResult.Data);
        }

        /// <summary>
        /// Gets all blocks success failure no items.
        /// </summary>
        [Test]
        public async Task GetAllBlocks_SuccessFailure_NoItems()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var mockContent = GetMockPublishedContentItem(true);
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var contentHandler = GetHandler(_config);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedChildrenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<IPublishedContent>());

            // Act
            var contentResult = await contentHandler.GetAllBlocksAsync();

            // Assert
            Assert.NotNull(contentResult);
            Assert.Empty(contentResult.Data);
        }

        /// <summary>
        /// Gets all blocks failure no folder unique identifier.
        /// </summary>
        [Test]
        public async Task GetAllBlocks_Failure_NoFolderGuid()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var mockContent = GetMockPublishedContentItem(true);
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", string.Empty } };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var contentHandler = GetHandler(_config);

            // Act
            var contentResult = await contentHandler.GetAllBlocksAsync();

            // Assert
            Assert.NotNull(contentResult);
            Assert.Empty(contentResult.Data);
        }

        #endregion

        #region Setup

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        private FutureNhsBlockHandler GetHandler(IConfiguration? config)
        {
            var handler = new FutureNhsBlockHandler(
                _mockFutureNhsContentService.Object, _mockFutureNhsBlockService.Object,
                config);

            return handler;
        }

        /// <summary>
        /// Gets the mock published content item.
        /// </summary>
        /// <returns></returns>
        private Mock<IPublishedContent> GetMockPublishedContentItem(bool isPublished)
        {
            var mockContent = new Mock<IPublishedContent>();
            mockContent.Setup(x => x.IsPublished(It.IsAny<string>())).Returns(isPublished);
            return mockContent;
        }

        #endregion
    }
}