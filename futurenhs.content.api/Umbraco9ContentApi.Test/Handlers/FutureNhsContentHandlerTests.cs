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
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using ContentModel = Core.Models.Content.ContentModel;

    /// <summary>
    /// Futrue Nhs Content Handler Tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsContentHandlerTests
    {
        // Mock Services
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService = new();
        private Mock<IFutureNhsBlockService> _mockFutureNhsBlockService = new();

        private IConfiguration _config;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentService = new Mock<IFutureNhsContentService>();
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();
        }

        [Test]
        public async Task DiscardDraftContent_BlocksAdded_Success()
        {
            // Arrange          
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);
            var mockContentModelWithBlocks = GetMockContentModelWithBlocks(true);
            var mockContentModelWithoutBlocks = GetMockContentModelWithBlocks(false);

            _mockFutureNhsContentService
                .Setup(x => x.GetDraftContent(It.IsAny<Guid>(), cancellationToken))
                .Returns(mockDraftContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedContent(It.IsAny<Guid>(), cancellationToken))
                .Returns(mockPublishedContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.ResolveDraftContent(It.IsAny<IContent>(), cancellationToken))
                .Returns(mockContentModelWithBlocks.Object);

            _mockFutureNhsContentService
                .Setup(x => x.ResolvePublishedContent(It.IsAny<IPublishedContent>(), It.IsAny<string>(), cancellationToken))
                .Returns(mockContentModelWithoutBlocks.Object);

            var contentHandler = GetHandler(_config, _mockFutureNhsContentService.Object, _mockFutureNhsBlockService.Object);

            // Act
            var contentResult = contentHandler.DiscardDraftContent(contentId, cancellationToken);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.True(contentResult.Succeeded);
        }

        [Test]
        public async Task DiscardDraftContent_BlocksRemoved_Success()
        {
            // Arrange          
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);
            var mockContentModelWithBlocks = GetMockContentModelWithBlocks(true);
            var mockContentModelWithoutBlocks = GetMockContentModelWithBlocks(false);

            _mockFutureNhsContentService
                .Setup(x => x.GetDraftContent(It.IsAny<Guid>(), cancellationToken))
                .Returns(mockDraftContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedContent(It.IsAny<Guid>(), cancellationToken))
                .Returns(mockPublishedContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.ResolveDraftContent(It.IsAny<IContent>(), cancellationToken))
                .Returns(mockContentModelWithoutBlocks.Object);

            _mockFutureNhsContentService
                .Setup(x => x.ResolvePublishedContent(It.IsAny<IPublishedContent>(), It.IsAny<string>(), cancellationToken))
                .Returns(mockContentModelWithBlocks.Object);

            var contentHandler = GetHandler(_config, _mockFutureNhsContentService.Object, _mockFutureNhsBlockService.Object);

            // Act
            var contentResult = contentHandler.DiscardDraftContent(contentId, cancellationToken);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.True(contentResult.Succeeded);
        }



        #region Setup

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="futureNhsContentService">The future NHS content service.</param>
        /// <returns></returns>
        private FutureNhsContentHandler GetHandler(IConfiguration config, IFutureNhsContentService futureNhsContentService, IFutureNhsBlockService futureNhsBlockService)
        {
            var handler = new FutureNhsContentHandler(
                futureNhsContentService, futureNhsBlockService);

            return handler;
        }

        /// <summary>
        /// Gets the mock content model.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        private ContentModel GetMockBlockContentModel(string contentType)
        {
            var dict = new Dictionary<string, object>()
            {
                { "Field", "Value" },
            };

            var mockContentModel = new Mock<ContentModel>();
            mockContentModel.Setup(x => x.Item.Id).Returns(Guid.NewGuid());
            mockContentModel.Setup(x => x.Item.ContentType).Returns(contentType);
            mockContentModel.Setup(x => x.Content).Returns(dict);
            return mockContentModel.Object;
        }

        private Mock<ContentModel> GetMockContentModelWithBlocks(bool withBlocks)
        {
            List<ContentModel> blockContentModels = new();

            if (withBlocks)
            {
                blockContentModels.Add(GetMockBlockContentModel(TextBlock.ModelTypeAlias));
                blockContentModels.Add(GetMockBlockContentModel(KeyLinksBlock.ModelTypeAlias));
            }

            var dict = new Dictionary<string, object>()
            {
                { "blocks", blockContentModels },
            };

            var mockPageContentModel = new Mock<ContentModel>();
            mockPageContentModel.Setup(x => x.Item.Id).Returns(Guid.NewGuid());
            mockPageContentModel.Setup(x => x.Item.ContentType).Returns(GeneralWebPage.ModelTypeAlias);
            mockPageContentModel.Setup(x => x.Content).Returns(dict);
            return mockPageContentModel;
        }

        /// <summary>
        /// Gets the mock content item.
        /// </summary>
        /// <returns></returns>
        private Mock<IContent> GetMockContent(Guid contentId)
        {
            var mockContent = new Mock<IContent>();
            mockContent.Setup(x => x.Key).Returns(contentId);
            mockContent.Setup(x => x.Properties.GetEnumerator()).Returns(GetPropertyCollection("blocks", "mainText").GetEnumerator());
            mockContent.Setup(x => x.ContentType.Alias).Returns(GeneralWebPage.ModelTypeAlias);
            return mockContent;
        }

        /// <summary>
        /// Gets the mock property.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        private IPropertyCollection GetPropertyCollection(params string[] alias)
        {
            List<IProperty> propertyList = new();
            Mock<IPropertyCollection> mockPropertyCollection = new();

            for (int i = 0; i < alias.Length; i++)
            {
                var mockProperty = new Mock<IProperty>();
                mockProperty.Setup(x => x.Alias).Returns(alias[i]);
                propertyList.Add(mockProperty.Object);
            }

            mockPropertyCollection.Setup(x => x.GetEnumerator()).Returns(propertyList.GetEnumerator());
            return mockPropertyCollection.Object;
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