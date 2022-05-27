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
    using System.Threading;
    using System.Threading.Tasks;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;
    using Assert = Xunit.Assert;

    /// <summary>
    /// Template api controller tests.
    /// </summary>
    [TestFixture]
    public class TemplateApiControllerTests
    {
        private Mock<IFutureNhsContentHandler> _mockFutureNhsContentHandler;
        private Mock<IFutureNhsTemplateHandler> _mockFutureNhsTemplateHandler;
        private CancellationToken cancellationToken;

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
            _mockFutureNhsTemplateHandler.Setup(x => x.GetAllTemplates(cancellationToken)).Returns(contentList);
            var controller = GetController();

            // Act
            var result = controller.GetAllTemplates(cancellationToken);
            var itemResult = result as OkObjectResult;
            var payloadResult = itemResult.Value as ApiResponse<IEnumerable<ContentModelData>>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.NotNull(itemResult.StatusCode);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(payloadResult.Data.FirstOrDefault().Content);
            var field = Assert.IsType<KeyValuePair<string, object>>(payloadResult.Data.FirstOrDefault().Content.FirstOrDefault());
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
            _mockFutureNhsTemplateHandler.Setup(x => x.GetAllTemplates(cancellationToken)).Returns(GetTestTemplates_NotFound());
            var controller = GetController();

            // Act
            var result = controller.GetAllTemplates(cancellationToken);
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
            _mockFutureNhsTemplateHandler.Setup(x => x.GetTemplate(It.IsAny<Guid>(), cancellationToken)).Returns(GetTemplate_Found(contentId));

            // Act
            var result = controller.GetTemplate(contentId, cancellationToken);
            var itemResult = result as OkObjectResult;
            var payloadResult = itemResult.Value as ApiResponse<ContentModelData>;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(payloadResult.Data.Content);
            var field = Assert.IsType<KeyValuePair<string, object>>(payloadResult.Data.Content.FirstOrDefault());
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
            _mockFutureNhsTemplateHandler.Setup(x => x.GetTemplate(It.IsAny<Guid>(), cancellationToken)).Returns(() => null);
            var controller = GetController();
            var contentId = new Guid("8E87CC7B-26BD-4543-906D-53652F5B6F02");

            // Act
            var result = controller.GetTemplate(contentId, cancellationToken);
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
        private TemplateController GetController()
        {
            var controller = new TemplateController(
                _mockFutureNhsContentHandler.Object,
                _mockFutureNhsTemplateHandler.Object);

            return controller;
        }

        /// <summary>
        /// Gets the test model.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<ContentModelData> GetTemplate_Found(Guid contentId)
        {
            var mockDictionary = new Dictionary<string, object>()
            {
                { "Title", "This is a title." }
            };

            var model = new ContentModelData()
            {
                Item = new ContentModelItemData
                {
                    Id = contentId,
                },
                Content = mockDictionary
            };

            var apiResponse = new Mock<ApiResponse<ContentModelData>>();
            apiResponse.Setup(x => x.Data).Returns(model);

            return apiResponse.Object;
        }

        // <summary>
        /// Gets the test model.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<IEnumerable<ContentModelData>> GetTestTemplates_Found()
        {
            var mockDictionary = new Dictionary<string, object>()
            {
                { "Title", "This is a title." }
            };

            var model = new List<ContentModelData>()
            {
                new ContentModelData() {Content = mockDictionary}
            };

            var apiResponse = new Mock<ApiResponse<IEnumerable<ContentModelData>>>();
            apiResponse.Setup(x => x.Data).Returns(model);
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }

        private ApiResponse<IEnumerable<ContentModelData>> GetTestTemplates_NotFound()
        {
            var model = new List<ContentModelData>()
            {

            };

            var apiResponse = new Mock<ApiResponse<IEnumerable<ContentModelData>>>();
            apiResponse.Setup(x => x.Data).Returns(model);
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }

        #endregion
    }
}