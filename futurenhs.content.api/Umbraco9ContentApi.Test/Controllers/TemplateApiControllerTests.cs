namespace Umbraco9ContentApi.Test.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Core.Controllers;
    using Core.Handlers.FutureNhs.Interface;
    using Core.Services.FutureNhs.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
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
            var contentList = GetTestBlocks();
            _mockFutureNhsTemplateHandler.Setup(x => x.GetAllTemplatesAsync()).ReturnsAsync(contentList);
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
        /// Gets all templates not found.
        /// </summary>
        [Test]
        public async Task GetAllTemplates_NotFound()
        {
            // Arrange
            _mockFutureNhsTemplateHandler.Setup(x => x.GetAllTemplatesAsync()).ReturnsAsync(new List<ContentModel>());
            var controller = GetController();

            // Act
            var result = await controller.GetAllAsync();
            var itemResult = result as NotFoundResult;


            // Assert
            Assert.NotNull(itemResult);
            Assert.Equal((int)HttpStatusCode.NotFound, itemResult.StatusCode);
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
            var content = GetTestModel();
            _mockFutureNhsTemplateHandler.Setup(x => x.GetTemplateAsync(It.IsAny<Guid>())).ReturnsAsync(content);

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
            var result = await controller.GetAsync(contentId);
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

        #endregion
    }
}