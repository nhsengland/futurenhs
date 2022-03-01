namespace Umbraco9ContentApi.Test.Handler
{
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Core.Handlers.FutureNhs;
    using Core.Services.FutureNhs.Interface;

    /// <summary>
    /// Futrue Nhs Content Handler Tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsContentHandlerTests
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

        #region Create Content Tests

        /// <summary>
        /// Creates the content page name success.
        /// </summary>
        [Test]
        public async Task CreateContent_PageName_Success()
        {
            // Arrange
            var newPageName = "Test Page";
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var mockContent = GetMockContentItem(contentId);
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var contentHandler = GetHandler(_config);

            _mockFutureNhsContentService
                .Setup(x => x.CreateAsync(It.IsAny<Guid>(),
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockContent.Object);

            // Act
            var contentResult = await contentHandler.CreateContentAsync(newPageName);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(contentResult.Key, contentId);
        }

        /// <summary>
        /// Creates the content no page name fail.
        /// </summary>
        [Test]
        public async Task CreateContent_NoPageName_Fail()
        {
            // Arrange
            var contentHandler = GetHandler(null);

            // Act
            var contentResult = await contentHandler.CreateContentAsync(string.Empty);

            // Assert
            Assert.IsNull(contentResult);
        }

        #endregion

        #region Update Content Tests

        #region Update Content Success Tests

        /// <summary>
        /// Updates the content all information provided unpublished success.
        /// </summary>
        [Test]
        public async Task UpdateContent_AllInformationProvided_Published_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var newDescription = "Update Description";
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, newDescription, newPageContent);

            // Assert
            Assert.IsTrue(contentResult);
        }

        /// <summary>
        /// Updates the content title provided published success.
        /// </summary>
        [Test]
        public async Task UpdateContent_TitleProvided_Published_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, string.Empty, string.Empty);

            // Assert
            Assert.IsTrue(contentResult);
        }

        /// <summary>
        /// Updates the content description provided published success.
        /// </summary>
        [Test]
        public async Task UpdateContent_DescriptionProvided_Published_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newDescription = "Update Description";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, newDescription, string.Empty);

            // Assert
            Assert.IsTrue(contentResult);
        }

        /// <summary>
        /// Updates the content content provided published success.
        /// </summary>
        [Test]
        public async Task UpdateContent_ContentProvided_Published_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, string.Empty, newPageContent);

            // Assert
            Assert.IsTrue(contentResult);
        }

        /// <summary>
        /// Updates the content all information provided unpublished success.
        /// </summary>
        [Test]
        public async Task UpdateContent_AllInformationProvided_Unpublished_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var newDescription = "Update Description";
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, newDescription, newPageContent);

            // Assert
            Assert.IsTrue(contentResult);
        }

        /// <summary>
        /// Updates the content title provided unpublished success.
        /// </summary>
        [Test]
        public async Task UpdateContent_TitleProvided_Unpublished_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, string.Empty, string.Empty);

            // Assert
            Assert.IsTrue(contentResult);
        }

        /// <summary>
        /// Updates the content description provided unpublished success.
        /// </summary>
        [Test]
        public async Task UpdateContent_DescriptionProvided_Unpublished_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newDescription = "Update Description";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, newDescription, string.Empty);

            // Assert
            Assert.IsTrue(contentResult);
        }

        /// <summary>
        /// Updates the content content provided unpublished success.
        /// </summary>
        [Test]
        public async Task UpdateContent_ContentProvided_Unpublished_Success()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(true);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, string.Empty, newPageContent);

            // Assert
            Assert.IsTrue(contentResult);
        }

        #endregion

        #region Update Content Fail Tests

        /// <summary>
        /// Updates the content no information provided fail.
        /// </summary>
        [Test]
        public async Task UpdateContent_NoInformationProvided_Fail()
        {
            // Arrange
            var contentHandler = GetHandler(null);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(Guid.NewGuid(), string.Empty, string.Empty, string.Empty);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content all information provided published failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_AllInformationProvided_Published_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var newDescription = "Update Description";
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, newDescription, newPageContent);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content title provided published failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_TitleProvided_Published_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, string.Empty, string.Empty);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content description provided published failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_DescriptionProvided_Published_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newDescription = "Update Description";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, newDescription, string.Empty);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content content provided published failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_ContentProvided_Published_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(true);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAndPublishAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, string.Empty, newPageContent);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content all information provided unpublished failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_AllInformationProvided_Unpublished_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var newDescription = "Update Description";
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, newDescription, newPageContent);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content title provided unpublished failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_TitleProvided_Unpublished_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageName = "Update Title";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, newPageName, string.Empty, string.Empty);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content description provided unpublished failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_DescriptionProvided_Unpublished_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newDescription = "Update Description";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, newDescription, string.Empty);

            // Assert
            Assert.IsFalse(contentResult);
        }

        /// <summary>
        /// Updates the content content provided unpublished failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_ContentProvided_Unpublished_Failure()
        {
            // Arrange
            var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
            var parentId = new Guid("519C62B9-6624-4CD8-9E8C-14432AC4F6DC");
            var newPageContent = "{blocks:{testBlock}}";
            var mockContent = GetMockContentItem(contentId);
            var mockPublishedContent = GetMockPublishedContentItem(false);

            var contentHandler = GetHandler(null);

            _mockFutureNhsContentService
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockContent.Object);

            _mockFutureNhsContentService
                .Setup(x => x.GetPublishedAsync(It.IsAny<Guid>())).ReturnsAsync(mockPublishedContent.Object);

            _mockFutureNhsContentService.Setup(x => x.SaveAsync(It.IsAny<IContent>()))
                .ReturnsAsync(false);

            // Act
            var contentResult = await contentHandler.UpdateContentAsync(parentId, string.Empty, string.Empty, newPageContent);

            // Assert
            Assert.IsFalse(contentResult);
        }

        #endregion

        #endregion

        #region Setup

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        private FutureNhsContentHandler GetHandler(IConfiguration? config)
        {
            var handler = new FutureNhsContentHandler(
                _mockFutureNhsContentService.Object,
                config);

            return handler;
        }

        /// <summary>
        /// Gets the mock content item.
        /// </summary>
        /// <returns></returns>
        private Mock<IContent> GetMockContentItem(Guid contentId)
        {
            var mockContent = new Mock<IContent>();
            mockContent.Setup(x => x.Key).Returns(contentId);
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