namespace Umbraco9ContentApi.Test.Controller
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Core.Controllers;
    using Core.Handlers.FutureNhs.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
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
        public async Task GetAllBlocks_Success()
        {
            // Arrange
            var contentList = GetTestBlocks();
            _mockFutureNhsBlockHandler.Setup(x => x.GetAllBlocks()).ReturnsAsync(contentList);
            var controller = GetController();

            // Act
            var result = await controller.GetAll();
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
        /// Gets all blocks not found.
        /// </summary>
        [Test]
        public async Task GetAllBlocks_NotFound()
        {
            // Arrange
            _mockFutureNhsBlockHandler.Setup(x => x.GetAllBlocks()).ReturnsAsync(new List<ContentModel>());
            var controller = GetController();

            // Act
            var result = await controller.GetAll();
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

        #endregion
    }
}