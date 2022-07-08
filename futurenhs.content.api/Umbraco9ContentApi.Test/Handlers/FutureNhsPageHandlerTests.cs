namespace Umbraco9ContentApi.Test.Handler
{
    using Core.Handlers.FutureNhs;
    using Core.Services.FutureNhs.Interface;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Services.FutureNhs;
    using ContentModelData = Core.Models.Content.ContentModelData;

    /// <summary>
    /// Futrue Nhs Content Handler Tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsPageHandlerTests
    {
        // Mock Services
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService = new();
        private Mock<IContentTypeService> _mockContentTypeService = new();
        private Mock<IFutureNhsBlockService> _mockFutureNhsBlockService = new();
        private Mock<IContentService> _mockContentService = new();
        private Mock<IFutureNhsValidationService> _mockFutureNhsValidationService = new();
        private Mock<ILogger<FutureNhsBlockService>> _mockLogger = new();

        // Variables
        private IConfiguration _config;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentService.SetupAllProperties();
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();
        }

        #region Create Page Tests

        ///// <summary>
        ///// Creates the content page name success.
        ///// </summary>
        [Test]
        public async Task CreatePage_PageName_Success()
        {
            // Arrange
            var newPageName = "Test Page";
            var contentId = Guid.NewGuid();
            var mockContent = GetMockContent(contentId);
            var pageHandler = GetHandler(_config,
                _mockFutureNhsContentService.Object,
                _mockFutureNhsBlockService.Object,
                _mockFutureNhsValidationService.Object);

            _mockFutureNhsContentService
                .Setup(x => x.CreateContentFromTemplate(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                    .Returns(mockContent.Object);

            // Act
            var contentResult = pageHandler.CreatePage(newPageName, null, cancellationToken);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.True(contentResult.Succeeded);
            Assert.AreEqual(contentResult.Data, contentId.ToString());
        }

        #endregion

        #region Update Page Tests

        #region Update Page Success Tests

        /// <summary>
        /// Updates the page all information provided success.
        /// </summary>
        [Test]
        public async Task UpdatePage_AllInformationProvided_Success()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var pageModel = GetMockPageModel(new List<ContentModelData>() {
                GetMockContentModel(TextBlock.ModelTypeAlias),
                GetMockContentModel(KeyLinksBlock.ModelTypeAlias)
            });
            var mockPageContent = GetMockContent(contentId);
            var mockBlockContent = new Mock<IContent>();
            var mockPublishedContent = GetMockPublishedContentItem(true);

            _mockFutureNhsContentService
                .Setup(x => x.GetDraftContent(It.IsAny<Guid>(), cancellationToken))
                .Returns(mockPageContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.ResolveDraftContent(It.IsAny<IContent>()))
                .Returns(GetMockContentModelWithChildBlocks(KeyLinksBlock.ModelTypeAlias));

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedContent(It.IsAny<Guid>(), cancellationToken))
                .Returns(mockPublishedContent.Object);

            var contentHandler = GetHandler(_config,
                _mockFutureNhsContentService.Object,
                _mockFutureNhsBlockService.Object,
                _mockFutureNhsValidationService.Object);

            // Act
            var contentResult = contentHandler.UpdatePage(contentId, pageModel.Object, cancellationToken);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(contentId.ToString(), contentResult.Data);
            Assert.True(contentResult.Succeeded);
        }
        #endregion

        #region Update Page Fail Tests     


        #endregion

        #endregion

        #region Setup

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="futureNhsContentService">The future NHS content service.</param>
        /// <param name="blockService">The block service.</param>
        /// <param name="futureNhsValidationService">The future NHS validation service.</param>
        /// <returns></returns>
        private FutureNhsPageHandler GetHandler(IConfiguration config, IFutureNhsContentService futureNhsContentService,
             IFutureNhsBlockService blockService, IFutureNhsValidationService futureNhsValidationService)
        {
            var handler = new FutureNhsPageHandler(config,
                futureNhsContentService, blockService, futureNhsValidationService);

            return handler;
        }

        /// <summary>
        /// Gets the mock page model.
        /// </summary>
        /// <param name="pageBlocks">The page blocks.</param>
        /// <returns></returns>
        private Mock<PageModel> GetMockPageModel(List<ContentModelData> pageBlocks)
        {
            var mockPageModel = new Mock<PageModel>();
            mockPageModel.Setup(x => x.Blocks).Returns(pageBlocks);
            return mockPageModel;
        }

        /// <summary>
        /// Gets the mock content model.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        private ContentModelData GetMockContentModel(string contentType)
        {
            var dict = new Dictionary<string, object>()
            {
                { "Field", "Value" }
            };

            var mockContentModel = new Mock<ContentModelData>();
            mockContentModel.Setup(x => x.Item.Id).Returns(Guid.NewGuid());
            mockContentModel.Setup(x => x.Item.ContentType).Returns(contentType);
            mockContentModel.Setup(x => x.Content).Returns(dict);
            return mockContentModel.Object;
        }

        private ContentModelData GetMockContentModelWithChildBlocks(string contentType)
        {
            var dict = new Dictionary<string, object>()
            {
                { "Field", "Value" },
                { "blocks", GetMockContentModel(KeyLinksBlock.ModelTypeAlias) }
            };

            var mockContentModel = new Mock<ContentModelData>();
            mockContentModel.Setup(x => x.Item.Id).Returns(Guid.NewGuid());
            mockContentModel.Setup(x => x.Item.ContentType).Returns(contentType);
            mockContentModel.Setup(x => x.Content).Returns(dict);
            return mockContentModel.Object;
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
        /// Gets the mock content item.
        /// </summary>
        /// <returns></returns>
        private Mock<IContent> GetMockContent(Guid contentId)
        {
            var mockContent = new Mock<IContent>();
            mockContent.Setup(x => x.Key).Returns(contentId);
            mockContent.Setup(x => x.Properties.GetEnumerator()).Returns(GetPropertyCollection("blocks", "mainText").GetEnumerator());
            return mockContent;
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