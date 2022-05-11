using System;

namespace Umbraco9ContentApi.Test.Controller
{
    using Core.Handlers.FutureNhs.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Umbraco9ContentApi.Core.Controllers;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;
    using Assert = Xunit.Assert;

    [TestFixture]
    public class ContentControllerTests
    {
        private Mock<IFutureNhsContentHandler> _mockFutureNhsContentHandler;
        private CancellationToken cancellationToken;

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
            _mockFutureNhsContentHandler.Setup(x => x.GetPublishedContent(It.IsAny<Guid>(), cancellationToken)).Returns(GetContent_Found(contentId));

            // Act
            var result = controller.GetPublishedContent(contentId, cancellationToken);
            var itemResult = result as OkObjectResult;
            var modelItem = itemResult.Value as ApiResponse<ContentModel>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(modelItem.Data.Content);
            var field = Assert.IsType<KeyValuePair<string, object>>(modelItem.Data.Content.FirstOrDefault());
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
            _mockFutureNhsContentHandler.Setup(x => x.GetPublishedContent(It.IsAny<Guid>(), cancellationToken)).Returns(() => new ApiResponse<ContentModel>().Failure(null, null));
            var controller = GetController();
            var contentId = new Guid("8E87CC7B-26BD-4543-906D-53652F5B6F02");

            // Act
            var result = controller.GetPublishedContent(contentId, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
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
            _mockFutureNhsContentHandler.Setup(x => x.PublishContentAndAssociatedContent(It.IsAny<Guid>(), cancellationToken)).Returns(AlterContent_Response(contentId));

            // Act
            var result = controller.PublishContent(contentId, cancellationToken);
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
        public async Task PublishContent_Failure()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.PublishContentAndAssociatedContent(It.IsAny<Guid>(), cancellationToken)).Returns(GetContents_Failure());

            // Act
            var result = controller.PublishContent(contentId, cancellationToken);
            var itemResult = result as ObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, itemResult.StatusCode.Value);
            var problemDetails = Assert.IsType<ProblemDetails>(itemResult.Value);
            Assert.Equal($"Error publishing content: {contentId}", problemDetails.Detail);
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
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContent(It.IsAny<Guid>(), cancellationToken)).Returns(AlterContent_Response(contentId));

            // Act
            var result = controller.DeleteContent(contentId, cancellationToken);
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
        public async Task DeleteContent_Failure()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContent(It.IsAny<Guid>(), cancellationToken)).Returns(GetContents_Failure());

            // Act
            var result = controller.DeleteContent(contentId, cancellationToken);
            var itemResult = result as ObjectResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, itemResult.StatusCode.Value);
            var problemDetails = Assert.IsType<ProblemDetails>(itemResult.Value);
            Assert.Equal($"Error deleting content: {contentId}", problemDetails.Detail);
        }


        #endregion

        #region Setup

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <returns></returns>
        private ContentController GetController()
        {
            var controller = new ContentController(
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
                Item = new ContentModelItem
                {
                    Id = contentId,
                },
                Content = mockDictionary
            };

            var apiResponse = new Mock<ApiResponse<ContentModel>>();
            apiResponse.Setup(x => x.Data).Returns(model);
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }

        /// <summary>
        /// Gets the content failure.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<string> GetContents_Failure()
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
                new ContentModel() {Content = mockDictionary}
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

        #endregion
    }
}