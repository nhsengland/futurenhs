namespace Umbraco9ContentApi.Test.Handler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Handlers.FutureNhs;
    using Core.Services.FutureNhs.Interface;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using UmbracoContentApi.Core.Models;
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;

    /// <summary>
    /// Future Nhs Template Handler tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsTemplateHandlerTests
    {
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService;
        private IConfiguration? _config;

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
            _mockFutureNhsContentService.Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(publishedContent.Object);

            // Act
            var result = handler.GetTemplateAsync(contentId);

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
                .Setup(x => x.GetPublishedChildrenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<IPublishedContent>()
                {
                    mockContent.Object
                });

            _mockFutureNhsContentService.SetupSequence(x => x.ResolveAsync(It.IsAny<IPublishedContent>()).Result)
                .Returns(new ContentModel() { System = new SystemModel() { Id = contentId } });

            // Act
            var contentResult = await contentHandler.GetAllTemplatesAsync();

            // Assert
            Assert.NotNull(contentResult);
            Assert.IsNotEmpty(contentResult);
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
        private UmbracoContentApi.Core.Models.ContentModel GetTestModel()
        {
            var mockDictionary = new Dictionary<string, object>()
            {
                { "Title", "This is a title." }
            };

            var model = new ContentModel()
            {
                Fields = mockDictionary
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
            return mockContent;
        }

        #endregion
    }
}