namespace Umbraco9ContentApi.Test.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Core.Controllers.Base;
    using Core.Handlers.FutureNhs.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
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
            var content = GetTestModel();
            _mockFutureNhsContentHandler.Setup(x => x.GetContentAsync(It.IsAny<Guid>())).ReturnsAsync(content);

            // Act
            var result = await controller.Get(contentId);
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
        /// Deletes the item success.
        /// </summary>
        [Test]
        public async Task DeleteItem_Success()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            var controller = GetController();
            var contentId = new Guid("A90E7522-18B4-444F-A736-0422A85C0D52");

            // Act
            var result = await controller.Delete(contentId);
            var itemResult = result as OkObjectResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.Equal($"Successfully deleted: {contentId}", itemResult.Value);
        }

        /// <summary>
        /// Deletes the item failure.
        /// </summary>
        [Test]
        public async Task DeleteItem_Failure()
        {
            // Arrange
            _mockFutureNhsContentHandler.Setup(x => x.DeleteContentAsync(It.IsAny<Guid>())).ReturnsAsync(false);
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

        #endregion
    }
}