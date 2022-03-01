using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Umbraco9ContentApi.Test.Controller
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Core.Controllers;
    using Core.Handlers.FutureNhs.Interface;
    using Core.Models.Request;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
    using Umbraco.Cms.Core.Models;
    using UmbracoContentApi.Core.Converters;
    using Assert = Xunit.Assert;
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;

    [TestFixture]
    public class ContentApiControllerTests
    {
        private Mock<IFutureNhsContentHandler> _mockFutureNhsContentHandler;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentHandler = new Mock<IFutureNhsContentHandler>();
        }

        #region Get Content Tests

        /// <summary>
        /// Gets the Content success.
        /// </summary>
        [Test]
        public async Task GetContent_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var content = GetTestModel();
            _mockFutureNhsContentHandler.Setup(x => x.GetContentAsync(It.IsAny<Guid>())).ReturnsAsync(content);

            // Act
            var result = await controller.GetAsync(contentId);
            var itemResult = result as OkObjectResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            var contentModel = Assert.IsType<ContentModel>(itemResult.Value);
            Assert.NotNull(contentModel.Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(contentModel.Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets the Content failure.
        /// </summary>
        [Test]
        public async Task GetContent_Failure()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.GetContentAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            var controller = GetController();
            var contentId = new Guid("8E87CC7B-26BD-4543-906D-53652F5B6F02");

            // Act
            var result = await controller.GetAsync(contentId);
            var itemResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.NotFound, itemResult.StatusCode);
        }

        #endregion

        #region Get All Content Tests

        /// <summary>
        /// Gets all content success.
        /// </summary>
        [Test]
        public async Task GetAllContent_Success()
        {
            // Arrange
            var contentList = GetTestBlocks();
            _mockFutureNhsContentHandler.Setup(x => x.GetAllContentAsync()).ReturnsAsync(contentList);
            var controller = GetController();

            // Act
            var result = await controller.GetAllAsync();
            var itemResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.NotNull(itemResult.StatusCode);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            var returnedList = Assert.IsType<List<ContentModel>>(itemResult.Value);
            var contentModel = Assert.IsType<ContentModel>(returnedList.FirstOrDefault());
            Assert.NotNull(contentModel.Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(contentModel.Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets all content not found.
        /// </summary>
        [Test]
        public async Task GetAllContent_NotFound()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.GetAllContentAsync()).ReturnsAsync(new List<ContentModel>());
            var controller = GetController();

            // Act
            var result = await controller.GetAllAsync();
            var itemResult = result as NotFoundResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.NotFound, itemResult.StatusCode);
        }

        #endregion

        #region Create Content Tests

        /// <summary>
        /// Creates the content success.
        /// </summary>
        [Test]
        public async Task CreateContent_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var content = GetMockContentItem(contentId);
            var pageName = "New Page";
            _mockFutureNhsContentHandler.Setup(
                x => x.CreateContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(content.Object);

            // Act
            var result = await controller.CreateAsync(pageName);
            var itemResult = result as OkObjectResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal(content.Object.Key, itemResult.Value);
        }

        /// <summary>
        /// Creates the name of the content failure no page.
        /// </summary>
        [Test]
        public async Task CreateContent_FailureNoPageName()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var content = GetMockContentItem(contentId);
            _mockFutureNhsContentHandler.Setup(
                x => x.CreateContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(content.Object);

            // Act
            var result = await controller.CreateAsync(string.Empty);
            var itemResult = result as BadRequestObjectResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, itemResult.StatusCode);
            Assert.Equal("Page name not provided, please provide a page name.", itemResult.Value);
        }

        /// <summary>
        /// Creates the content failure content create fail.
        /// </summary>
        [Test]
        public async Task CreateContent_FailureContentCreateFail()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var content = GetMockContentItem(contentId);
            var pageName = "New Page";
            _mockFutureNhsContentHandler.Setup(x => 
                x.CreateContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => null);

            // Act
            var result = await controller.CreateAsync(pageName);
            var itemResult = result as ObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, itemResult.StatusCode.Value);
            var problemDetails = Assert.IsType<ProblemDetails>(itemResult.Value);
            Assert.Equal("Error creating the page, content was null.", problemDetails.Detail);
        }

        #endregion

        #region Publish Content Tests

        /// <summary>
        /// Publishes the content success.
        /// </summary>
        [Test]
        public async Task PublishContent_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.PublishContentAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await controller.PublishAsync(contentId);
            var itemResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal($"Successfully published: {contentId}", itemResult.Value);
        }

        /// <summary>
        /// Publishes the content failure.
        /// </summary>
        [Test]
        public async Task PublishContent_Failure()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.PublishContentAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            var result = await controller.PublishAsync(contentId);
            var itemResult = result as ObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, itemResult.StatusCode.Value);
            var problemDetails = Assert.IsType<ProblemDetails>(itemResult.Value);
            Assert.Equal($"Publish unsuccessful: {contentId}", problemDetails.Detail);
        }

        #endregion

        #region Update Content Tests

        /// <summary>
        /// Updates the content success.
        /// </summary>
        [Test]
        public async Task UpdateContent_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var updateRequest = GetUpdateRequest("Test Title", "Test Description", "Test Content");
            _mockFutureNhsContentHandler.Setup(x => x.UpdateContentAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await controller.UpdateAsync(contentId, updateRequest);
            var itemResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal($"Update successful: {contentId}", itemResult.Value);
        }

        /// <summary>
        /// Publishes the content failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_Failure()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var updateRequest = GetUpdateRequest("Test Title", "Test Description", "Test Content");
            _mockFutureNhsContentHandler.Setup(x => x.UpdateContentAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await controller.UpdateAsync(contentId, updateRequest);
            var itemResult = result as ObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, itemResult.StatusCode.Value);
            var problemDetails = Assert.IsType<ProblemDetails>(itemResult.Value);
            Assert.Equal($"Update unsuccessful: {contentId}", problemDetails.Detail);
        }

        [Test]
        public async Task UpdateContent_BadRequest()
        {
            // Arrange
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var controller = GetController();
            var updateRequest = GetUpdateRequest(null, null, null);

            // Act
            var result = await controller.UpdateAsync(contentId, updateRequest);
            var itemResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, itemResult.StatusCode.Value);
            Assert.Equal("No update provided, please check you are sending an update.", itemResult.Value);
        }

        #endregion

        #region Delete Content Tests

        /// <summary>
        /// Delete the content success.
        /// </summary>
        [Test]
        public async Task DeleteContent_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await controller.DeleteAsync(contentId);
            var itemResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal($"Successfully deleted: {contentId}", itemResult.Value);
        }

        /// <summary>
        /// Publishes the content failure.
        /// </summary>
        [Test]
        public async Task DeleteContent_Failure()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            var result = await controller.DeleteAsync(contentId);
            var itemResult = result as ObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, itemResult.StatusCode.Value);
            var problemDetails = Assert.IsType<ProblemDetails>(itemResult.Value);
            Assert.Equal($"Deletion unsuccessful: {contentId}", problemDetails.Detail);
        }

        #endregion

        #region Setup

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <returns></returns>
        private ContentApiController GetController()
        {
            var controller = new ContentApiController(
                _mockFutureNhsContentHandler.Object);

            return controller;
        }

        /// <summary>
        /// Gets the test model.
        /// </summary>
        /// <returns></returns>
        private ContentModel GetTestModel()
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
        /// Gets the test blocks.
        /// </summary>
        /// <returns></returns>
        private List<ContentModel> GetTestBlocks()
        {
            var mockDictionary = new Dictionary<string, object>()
            {
                { "Title", "This is a title." }
            };

            var list = new List<ContentModel>()
            {
                new ContentModel()
                {
                    Fields = mockDictionary
                }
            };

            return list;
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
        /// Gets the update request.
        /// </summary>
        /// <returns></returns>
        private GeneralWebPageUpdateRequest GetUpdateRequest(string? title, string? description, string? pageContent)
        {
            return new GeneralWebPageUpdateRequest()
            {
                Title = title,
                Description = description,
                PageContent = pageContent
            };
        }

        #endregion
    }
}