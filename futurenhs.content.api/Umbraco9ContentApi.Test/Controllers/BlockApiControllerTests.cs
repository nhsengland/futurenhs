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
    using System.Threading;
    using System.Threading.Tasks;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;
    using Assert = Xunit.Assert;

    /// <summary>
    /// Block API controller tests
    /// </summary>
    [TestFixture]
    public class BlockApiControllerTests
    {
        private Mock<IFutureNhsContentHandler> _mockFutureNhsContentHandler;
        private Mock<IFutureNhsBlockHandler> _mockFutureNhsBlockHandler;
        private CancellationToken cancellationToken;

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
            _mockFutureNhsBlockHandler.Setup(x => x.GetAllBlocksAsync(cancellationToken)).ReturnsAsync(GetBlocks_Found);
            var controller = GetController();

            // Act
            var result = await controller.GetAllBlocksAsync(cancellationToken);
            var itemResult = result as OkObjectResult;
            var payloadResult = itemResult.Value as ApiResponse<IEnumerable<ContentModel>>;

            // Assert
            Assert.NotNull(itemResult);
            Assert.NotNull(itemResult.StatusCode);
            Assert.Equal((int)HttpStatusCode.OK, itemResult.StatusCode.Value);
            Assert.NotNull(payloadResult.Data.FirstOrDefault().Content);
            var field = Assert.IsType<KeyValuePair<string, object>>(payloadResult.Data.FirstOrDefault().Content.FirstOrDefault());
            Assert.Equal("Title", field.Key);
            Assert.Equal("Test text.", field.Value);
        }

        /// <summary>
        /// Gets all blocks not found.
        /// </summary>
        [Test]
        public async Task GetAllBlocks_NotFoundAsync()
        {
            // Arrange
            _mockFutureNhsBlockHandler.Setup(x => x.GetAllBlocksAsync(cancellationToken)).ReturnsAsync(GetTestBlocks_NotFound());
            var controller = GetController();

            // Act
            var result = await controller.GetAllBlocksAsync(cancellationToken);
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
        private BlockController GetController()
        {
            var controller = new BlockController(
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
                { "Title", "Test text." }
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
        /// Gets the test blocks not found.
        /// </summary>
        /// <returns></returns>
        private ApiResponse<IEnumerable<ContentModel>> GetTestBlocks_NotFound()
        {
            var apiResponse = new Mock<ApiResponse<IEnumerable<ContentModel>>>();
            apiResponse.Setup(x => x.Data).Returns(new List<ContentModel>());
            apiResponse.Setup(x => x.Succeeded).Returns(true);

            return apiResponse.Object;
        }
        #endregion
    }
}