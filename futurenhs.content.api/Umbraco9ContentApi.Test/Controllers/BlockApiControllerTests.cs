namespace Umbraco9ContentApi.Test.Controller
{
    using Core.Controllers;
    using Core.Handlers.FutureNhs.Interface;
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

    /// <summary>
    /// Block API controller tests
    /// </summary>
    [TestFixture]
    public class BlockApiControllerTests
    {
        private Mock<IFutureNhsContentHandler> _mockFutureNhsContentHandler;
        private Mock<IFutureNhsBlockHandler> _mockFutureNhsBlockHandler;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentHandler = new Mock<IFutureNhsContentHandler>();
            _mockFutureNhsBlockHandler = new Mock<IFutureNhsBlockHandler>();
        }

        #region Get All Blocks Tests

        /// <summary>
        /// Gets all blocks success.
        /// </summary>
        [Test]
        public async Task GetAllBlocks_SuccessAsync()
        {
            // Arrange
            _mockFutureNhsBlockHandler.Setup(x => x.GetAllBlocksAsync()).ReturnsAsync(GetBlocks_Found);
            var controller = GetController();

            // Act
            var result = await controller.GetAllBlocksAsync();
            var itemResult = result as OkObjectResult;
            var payloadResult = itemResult.Value as ApiResponse<IEnumerable<ContentModel>>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.NotNull(itemResult.StatusCode);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(payloadResult.Payload.FirstOrDefault().Fields);
            var field = Assert.IsType<KeyValuePair<string, object>>(payloadResult.Payload.FirstOrDefault().Fields.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("This is a title.", field.Value);
        }

        /// <summary>
        /// Gets all blocks not found.
        /// </summary>
        [Test]
        public async Task GetAllBlocks_NotFoundAsync()
        {
            // Arrange
            _mockFutureNhsBlockHandler.Setup(x => x.GetAllBlocksAsync()).ReturnsAsync(GetTestBlocks_NotFound());
            var controller = GetController();

            // Act
            var result = await controller.GetAllBlocksAsync();
            var itemResult = result as NotFoundObjectResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.NotFound, itemResult.StatusCode.Value);
            Assert.Equal("No blocks found.", itemResult.Value);
        }

        #endregion

        #region Setup

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <returns></returns>
        private BlockApiController GetController()
        {
            var controller = new BlockApiController(
                _mockFutureNhsContentHandler.Object,
                _mockFutureNhsBlockHandler.Object);

            return controller;
        }

        /// <summary>
        /// Gets the test blocks found.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<IEnumerable<ContentModel>> GetBlocks_Found()
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
            apiResponse.Setup(x => x.Payload).Returns(model);
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }


        /// <summary>
        /// Gets the test blocks not found.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<IEnumerable<ContentModel>> GetTestBlocks_NotFound()
        {
            var apiResponse = new Mock<ApiResponse<IEnumerable<ContentModel>>>();
            apiResponse.Setup(x => x.Payload).Returns(new List<ContentModel>());
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }
        #endregion
    }
}