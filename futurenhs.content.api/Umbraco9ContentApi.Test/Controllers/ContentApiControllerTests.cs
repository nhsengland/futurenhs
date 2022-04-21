using System;

namespace Umbraco9ContentApi.Test.Controller
{
    using Core.Controllers;
    using Core.Handlers.FutureNhs.Interface;
    using Core.Models.Request;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Umbraco9ContentApi.Core.Models.Response;
    using UmbracoContentApi.Core.Models;
    using Assert = Xunit.Assert;

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
        public async Task GetContent_SuccessAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.GetContentAsync(It.IsAny<Guid>())).ReturnsAsync(GetContent_Found(contentId));

            // Act
            var result = await controller.GetContentAsync(contentId);
            var itemResult = result as OkObjectResult;
            var modelItem = itemResult.Value as ApiResponse<ContentModel>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(modelItem.Data.Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(modelItem.Data.Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets the Content failure.
        /// </summary>
        [Test]
        public async Task GetContent_FailureAsync()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.GetContentAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            var controller = GetController();
            var contentId = new Guid("8E87CC7B-26BD-4543-906D-53652F5B6F02");

            // Act
            var result = await controller.GetContentAsync(contentId);
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
        public async Task GetAllContent_SuccessAsync()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.GetAllContentAsync()).ReturnsAsync(GetContents_Found());
            var controller = GetController();

            // Act
            var result = await controller.GetAllContentAsync();
            var itemResult = result as OkObjectResult;
            var modelItem = itemResult.Value as ApiResponse<IEnumerable<ContentModel>>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.NotNull(itemResult.StatusCode);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(modelItem.Data.FirstOrDefault().Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(modelItem.Data.FirstOrDefault().Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets all content not found.
        /// </summary>
        [Test]
        public async Task GetAllContent_NotFoundAsync()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.GetAllContentAsync()).ReturnsAsync(GetContents_NotFound());
            var controller = GetController();

            // Act
            var result = await controller.GetAllContentAsync();
            var itemResult = result as NotFoundObjectResult;


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
        public async Task CreateContent_SuccessAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var createRequest = new GeneralWebPageCreateRequest()
            {
                Name = "New Page",
            };
            _mockFutureNhsContentHandler.Setup(
                x => x.CreateContentAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<bool>())).ReturnsAsync(AlterContent_Response(contentId));

            // Act
            var result = await controller.CreateContentAsync(createRequest);
            var itemResult = result as OkObjectResult;
            var modelResult = itemResult.Value as ApiResponse<string>;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal(contentId.ToString(), modelResult.Data);
        }

        /// <summary>
        /// Creates the name of the content failure no page.
        /// </summary>
        [Test]
        public async Task CreateContent_FailureNoPageNameAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var mockCreateResult = new Mock<ApiResponse<string>>();
            mockCreateResult.Setup(x => x.Data).Returns(contentId.ToString());
            _mockFutureNhsContentHandler.Setup(
                x => x.CreateContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(mockCreateResult.Object);

            // Act
            var result = await controller.CreateContentAsync(new GeneralWebPageCreateRequest());
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
        public async Task CreateContent_FailureContentCreateFailAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var content = GetContent_Found(contentId);
            _mockFutureNhsContentHandler.Setup(x =>
                x.CreateContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => GetContent_Failure());

            // Act
            var result = await controller.CreateContentAsync(new GeneralWebPageCreateRequest() { Name = "New Page" });
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
        public async Task PublishContent_SuccessAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.PublishContentAsync(It.IsAny<Guid>())).ReturnsAsync(AlterContent_Response(contentId));

            // Act
            var result = await controller.PublishContentAsync(contentId);
            var itemResult = result as ObjectResult;
            var modelItem = itemResult.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal(contentId.ToString(), modelItem.Data);
        }

        /// <summary>
        /// Publishes the content failure.
        /// </summary>
        [Test]
        public async Task PublishContent_FailureAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.PublishContentAsync(It.IsAny<Guid>())).ReturnsAsync(GetContent_Failure());

            // Act
            var result = await controller.PublishContentAsync(contentId);
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
        public async Task UpdateContent_SuccessAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var updateRequest = GetUpdateRequest("Test Title", "Test Description", "Test Content");
            _mockFutureNhsContentHandler.Setup(x => x.UpdateContentAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(AlterContent_Response(contentId));

            // Act
            var result = await controller.UpdateContentAsync(contentId, updateRequest);
            var itemResult = result as OkObjectResult;
            var modelItem = itemResult.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal(contentId.ToString(), modelItem.Data);
        }

        /// <summary>
        /// Publishes the content failure.
        /// </summary>
        [Test]
        public async Task UpdateContent_FailureAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var updateRequest = GetUpdateRequest("Test Title", "Test Description", "Test Content");
            _mockFutureNhsContentHandler.Setup(x => x.UpdateContentAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(GetContent_Failure());

            // Act
            var result = await controller.UpdateContentAsync(contentId, updateRequest);
            var itemResult = result as ObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, itemResult.StatusCode.Value);
            var problemDetails = Assert.IsType<ProblemDetails>(itemResult.Value);
            Assert.Equal($"Update unsuccessful: {contentId}", problemDetails.Detail);
        }

        [Test]
        public async Task UpdateContent_BadRequestAsync()
        {
            // Arrange
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            var controller = GetController();
            var updateRequest = GetUpdateRequest(null, null, null);

            // Act
            var result = await controller.UpdateContentAsync(contentId, updateRequest);
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
        public async Task DeleteContent_SuccessAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(AlterContent_Response(contentId));

            // Act
            var result = await controller.DeleteContentAsync(contentId);
            var itemResult = result as OkObjectResult;
            var modelItem = itemResult.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.True(modelItem.Succeeded);
            Assert.Equal(modelItem.Data, contentId.ToString());
        }

        /// <summary>
        /// Publishes the content failure.
        /// </summary>
        [Test]
        public async Task DeleteContent_FailureAsync()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(GetContent_Failure());

            // Act
            var result = await controller.DeleteContentAsync(contentId);
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
        /// Alters the content response.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        private ApiResponse<string> AlterContent_Response(Guid contentId)
        {
            var mock = new Mock<ApiResponse<string>>();
            mock.Setup(x => x.Succeeded).Returns(true);
            mock.Setup(x => x.Data).Returns(contentId.ToString());
            return mock.Object;
        }

        /// <summary>
        /// Gets the content found.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        private ApiResponse<ContentModel> GetContent_Found(Guid contentId)
        {
            var mockDictionary = new Dictionary<string, object>()
            {
                { "Title", "This is a title." }
            };

            var model = new ContentModel()
            {
                System = new SystemModel
                {
                    Id = contentId,
                },
                Fields = mockDictionary
            };

            var apiResponse = new Mock<ApiResponse<ContentModel>>();
            apiResponse.Setup(x => x.Data).Returns(model);

            return apiResponse.Object;
        }

        /// <summary>
        /// Gets the content failure.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<string> GetContent_Failure()
        {
            var mock = new Mock<ApiResponse<string>>();
            mock.Setup(x => x.Succeeded).Returns(false);
            mock.Setup(x => x.Message).Returns("Error creating the page, content was null.");
            return mock.Object;
        }

        /// <summary>
        /// Gets the test contents found.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<IEnumerable<ContentModel>> GetContents_Found()
        {
            var mockDictionary = new Dictionary<string, object>()
            {
                { "Title", "This is a title." }
            };

            var model = new List<ContentModel>()
            {
                new ContentModel() {Fields = mockDictionary}
            };

            var apiResponse = new Mock<ApiResponse<IEnumerable<ContentModel>>>();
            apiResponse.Setup(x => x.Data).Returns(model);
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }

        /// <summary>
        /// Gets the test contents not found.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<IEnumerable<ContentModel>> GetContents_NotFound()
        {
            var apiResponse = new Mock<ApiResponse<IEnumerable<ContentModel>>>();
            apiResponse.Setup(x => x.Data).Returns(new List<ContentModel>());
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }

        /// <summary>
        /// Gets the update request.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="pageContent">Content of the page.</param>
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