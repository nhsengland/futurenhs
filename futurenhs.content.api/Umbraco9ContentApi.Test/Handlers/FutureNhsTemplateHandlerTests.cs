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

    /// <summary>
    /// Future Nhs Template Handler tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsTemplateHandlerTests
    {
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService;
        private IConfiguration? _config;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentService = new Mock<IFutureNhsContentService>();
        }

        #region Get Template Tests

        [Test]
        public async Task GetTemplate_Success()
        {
            // Arrange
            var handler = GetHandler(null);
            var contentId = new Guid("A90E7522-18B4-444F-A736-0422A85C0D52");
            var publishedContent = GetMockPublishedContentItem(true);
            var content = GetTestModel();
            _mockFutureNhsContentService.Setup(x => x.GetPublishedContent(It.IsAny<Guid>(), cancellationToken)).Returns(publishedContent.Object);

            // Act
            var result = handler.GetTemplate(contentId, cancellationToken);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Get All Templates Tests

        /// <summary>
        /// Gets all templates success.
        /// </summary>
        [Test]
        public async Task GetAllTemplates_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var mockContent = GetMockPublishedContentItem(true);
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Templates", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var contentHandler = GetHandler(_config);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedContent(It.IsAny<Guid>(), cancellationToken))
                .Returns(mockContent.Object);

            _mockFutureNhsContentService.Setup(x => x.ResolvePublishedContent(It.IsAny<IPublishedContent>(), It.IsAny<string>()))
                .Returns(new ContentModelData() { Item = new ContentModelItemData() { Id = contentId } });

            // Act
            var contentResult = contentHandler.GetAllTemplates(cancellationToken);

            // Assert
            Assert.NotNull(contentResult);
            Assert.IsNotEmpty(contentResult.Data);
        }
        #endregion

        #region Setup

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        private FutureNhsTemplateHandler GetHandler(IConfiguration? config)
        {
            var handler = new FutureNhsTemplateHandler(
                _mockFutureNhsContentService.Object,
                config);

            return handler;
        }

        /// <summary>
        /// Gets the test model.
        /// </summary>
        /// <returns></returns>
        private ContentModelData GetTestModel()
        {
            var mockDictionary = new Dictionary<string, object>()
            {
                { "Title", "This is a title." }
            };

            var model = new ContentModelData()
            {
                Content = mockDictionary
            };

            return model;
        }

        /// <summary>
        /// Gets the mock published content item.
        /// </summary>
        /// <returns></returns>
        private Mock<IPublishedContent> GetMockPublishedContentItem(bool isPublished)
        {
            var mockContent = new Mock<IPublishedContent>();
            mockContent.Setup(x => x.IsPublished(It.IsAny<string>())).Returns(isPublished);
            mockContent.Setup(x => x.Children).Returns(new List<IPublishedContent> { mockContent.Object });
            return mockContent;
        }


        #endregion
    }
}