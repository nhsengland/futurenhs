namespace Umbraco9ContentApi.Test.Controller
{
    using Core.Controllers;
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
    /// Template api controller tests.
    /// </summary>
    [TestFixture]
    public class TemplateApiControllerTests
    {
        private Mock<IFutureNhsContentHandler> _mockFutureNhsContentHandler;
        private Mock<IFutureNhsTemplateHandler> _mockFutureNhsTemplateHandler;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentHandler = new Mock<IFutureNhsContentHandler>();
            _mockFutureNhsTemplateHandler = new Mock<IFutureNhsTemplateHandler>();
        }

        #region Get All Templates Tests

        /// <summary>
        /// Gets all templates success.
        /// </summary>
        [Test]
        public async Task GetAllTemplates_Success()
        {
            // Arrange
            var contentList = GetTestTemplates_Found();
            _mockFutureNhsTemplateHandler.Setup(x => x.GetAllTemplatesAsync()).ReturnsAsync(contentList);
            var controller = GetController();

            // Act
            var result = await controller.GetAllTemplatesAsync();
            var itemResult = result as OkObjectResult;
            var payloadResult = itemResult.Value as ApiResponse<IEnumerable<ContentModel>>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.NotNull(itemResult.StatusCode);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(payloadResult.Data.FirstOrDefault().Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(payloadResult.Data.FirstOrDefault().Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets all templates not found.
        /// </summary>
        [Test]
        public async Task GetAllTemplates_NotFound()
        {
            // Arrange
            _mockFutureNhsTemplateHandler.Setup(x => x.GetAllTemplatesAsync()).ReturnsAsync(GetTestTemplates_NotFound());
            var controller = GetController();

            // Act
            var result = await controller.GetAllTemplatesAsync();
            var itemResult = result as NotFoundObjectResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.NotFound, itemResult.StatusCode.Value);
            Assert.Equal("No templates found.", itemResult.Value);
        }

        #endregion

        #region Get Template Test

        /// <summary>
        /// Gets the Template success.
        /// </summary>
        [Test]
        public async Task GetTemplate_Success()
        {
            // Arrange
            var controller = GetController();
            var contentId = new Guid("4C8F8C9D-DF83-4815-BF63-1DE803903326");
            _mockFutureNhsTemplateHandler.Setup(x => x.GetTemplateAsync(It.IsAny<Guid>())).ReturnsAsync(GetTemplate_Found(contentId));

            // Act
            var result = await controller.GetTemplateAsync(contentId);
            var itemResult = result as OkObjectResult;
            var payloadResult = itemResult.Value as ApiResponse<ContentModel>;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(payloadResult.Data.Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(payloadResult.Data.Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets the Template failure.
        /// </summary>
        [Test]
        public async Task GetTemplate_Failure()
        {
            // Arrange
            _mockFutureNhsTemplateHandler.Setup(x => x.GetTemplateAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            var controller = GetController();
            var contentId = new Guid("8E87CC7B-26BD-4543-906D-53652F5B6F02");

            // Act
            var result = await controller.GetTemplateAsync(contentId);
            var itemResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.NotFound, itemResult.StatusCode);
        }

        #endregion

        #region Setup

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <returns></returns>
        private TemplateApiController GetController()
        {
            var controller = new TemplateApiController(
                _mockFutureNhsContentHandler.Object,
                _mockFutureNhsTemplateHandler.Object);

            return controller;
        }

        /// <summary>
        /// Gets the test model.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<ContentModel> GetTemplate_Found(Guid contentId)
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

        // <summary>
        /// Gets the test model.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<IEnumerable<ContentModel>> GetTestTemplates_Found()
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

        private ApiResponse<IEnumerable<ContentModel>> GetTestTemplates_NotFound()
        {
            var model = new List<ContentModel>()
            {

            };

            var apiResponse = new Mock<ApiResponse<IEnumerable<ContentModel>>>();
            apiResponse.Setup(x => x.Data).Returns(model);
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }

        #endregion
    }
}