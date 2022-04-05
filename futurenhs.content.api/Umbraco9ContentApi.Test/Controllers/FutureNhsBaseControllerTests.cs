namespace Umbraco9ContentApi.Test.Controller
{
    using Core.Controllers.Base;
    using Core.Handlers.FutureNhs.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Umbraco9ContentApi.Core.Models.Response;
    using UmbracoContentApi.Core.Models;
    using Assert = Xunit.Assert;

    /// <summary>
    /// Future Nhs Base Controller Tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsBaseControllerTests
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

        #region Get Tests

        /// <summary>
        /// Gets the item success.
        /// </summary>
        [Test]
        public async Task GetItem_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("A90E7522-18B4-444F-A736-0422A85C0D52");
            _mockFutureNhsContentHandler.Setup(x => x.GetContentAsync(It.IsAny<Guid>())).ReturnsAsync(GetContent_Found(contentId));

            // Act
            var result = await controller.Get(contentId);
            var itemResult = result as OkObjectResult;
            var modelItem = itemResult.Value as ApiResponse<ContentModel>;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(modelItem.Payload.Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(modelItem.Payload.Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets the item failure.
        /// </summary>
        [Test]
        public async Task GetItem_Failure()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.GetContentAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            var controller = GetController();
            var contentId = new Guid("100C7B19-AFD5-4624-B5C5-47E61ABB767A");

            // Act
            var result = await controller.Get(contentId);
            var itemResult = result as NoContentResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.NoContent, itemResult.StatusCode);
        }

        #endregion

        #region Delete Tests

        /// <summary>
        /// Delete the content success.
        /// </summary>
        [Test]
        public async Task DeleteItem_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(AlterContent_Success(contentId));

            // Act
            var result = await controller.Delete(contentId);
            var itemResult = result as OkObjectResult;
            var modelItem = itemResult.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.True(modelItem.Succeeded);
            Assert.Equal(modelItem.Payload, contentId.ToString());
        }

        /// <summary>
        /// Deletes the item failure.
        /// </summary>
        [Test]
        public async Task DeleteItem_Failure()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(AlterContent_Failure());
            var controller = GetController();
            var contentId = new Guid("6087DBA7-E10D-430D-B4F7-FC9408F893C0");

            // Act
            var result = await controller.Delete(contentId);
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
        private FutureNhsBaseController GetController()
        {
            var controller = new FutureNhsBaseController(
                _mockFutureNhsContentHandler.Object);

            return controller;
        }

        /// <summary>
        /// Alters the content success.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        private ApiResponse<string> AlterContent_Success(Guid contentId)
        {
            var mock = new Mock<ApiResponse<string>>();
            mock.Setup(x => x.Succeeded).Returns(true);
            mock.Setup(x => x.Payload).Returns(contentId.ToString());
            return mock.Object;
        }

        /// <summary>
        /// Alters the content failure.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<string> AlterContent_Failure()
        {
            var mock = new Mock<ApiResponse<string>>();
            mock.Setup(x => x.Succeeded).Returns(false);
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
            apiResponse.Setup(x => x.Payload).Returns(model);

            return apiResponse.Object;
        }

        #endregion
    }
}