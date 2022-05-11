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
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;

    /// <summary>
    /// Futrue Nhs Content Handler Tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsPageHandlerTests
    {
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService;
        private Mock<IFutureNhsValidationService> _mockFutureNhsValidationService;
        private Mock<IContentTypeService> _mockContentTypeService;
        private IConfiguration? _config;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentService = new Mock<IFutureNhsContentService>();
            _mockFutureNhsValidationService = new Mock<IFutureNhsValidationService>();
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();
        }

        #region Create Content Tests

        ///// <summary>
        ///// Creates the content page name success.
        ///// </summary>
        //[Test]
        //public async Task CreatePage_PageName_Success()
        //{
        //    // Arrange
        //    var newPageName = "Test Page";
        //    var contentId = new Guid("81D3DB69-62FF-4549-824D-25A4B9F37626");
        //    var mockContent = GetMockContentItem(contentId);
        //    var pageHandler = GetHandler(_config);

        //    _mockFutureNhsContentService
        //        .Setup(x => x.CreateContentAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), cancellationToken))
        //        .ReturnsAsync(mockContent.Object);

        //    // Act
        //    var contentResult = await pageHandler.CreatePageAsync(newPageName, null, cancellationToken);

        //    // Assert
        //    Assert.IsNotNull(contentResult);
        //    Assert.AreEqual(contentResult.Data, contentId.ToString());
        //}

        #endregion

        #region Update Page Tests

        #region Update Page Success Tests

        //TODO

        #endregion

        #region Update Page Fail Tests

        //TODO

        #endregion

        #endregion

        #region Setup

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        private FutureNhsPageHandler GetHandler(IConfiguration config)
        {
            var handler = new FutureNhsPageHandler(config,
                _mockFutureNhsContentService.Object, _mockFutureNhsValidationService.Object, _mockContentTypeService.Object);

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