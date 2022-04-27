using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco9ContentApi.Core.Handlers.FutureNhs;
using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;


namespace Umbraco9ContentApi.Test
{
    /// <summary>
    /// FutureNhsContentHandlerTests
    /// </summary>
    public class FutureNhsContentHandlerTests
    {
        // handler
        private IFutureNhsContentHandler _contentHandler;

        // mocks
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService;
        private Mock<IFutureNhsValidationService> _mockFutureNhsValidaitonService;
        private IConfiguration? _config;

        // variables
        private readonly List<IPublishedContent> _publishedContentList = new List<IPublishedContent>();
        private readonly List<Guid> _guidList = new List<Guid>();
        private Guid _guid;


        [SetUp]
        public void Setup()
        {
            _mockFutureNhsContentService = new Mock<IFutureNhsContentService>();
            _mockFutureNhsValidaitonService = new Mock<IFutureNhsValidationService>();
        }

        private void CustomContentHandlerSetup()
        {
            _contentHandler = new FutureNhsContentHandler(
                _config,
                _mockFutureNhsContentService.Object,
                _mockFutureNhsValidaitonService.Object);
        }

        private IEnumerable<IPublishedContent> GenerateContentModels()
        {
            for (int i = 0; i < 4; i++)
            {
                var guid = Guid.NewGuid();
                _guidList.Add(guid);
                Mock<IPublishedContent> mock = new Mock<IPublishedContent>();
                mock.Setup(x => x.Key).Returns(guid);
                _publishedContentList.Add(mock.Object);
            }

            return _publishedContentList;
        }

        [Test]
        public void GetContent_Test()
        {
            // Arrange
            _guid = Guid.NewGuid();
            _mockFutureNhsContentService.Setup(x => x.ResolveAsync(It.IsAny<IPublishedContent>()).Result).Returns(new ContentModel() { Item = new ItemModel() { Id = _guid } });
            CustomContentHandlerSetup();

            // Act
            var result = _contentHandler.GetContentAsync(_guid).Result;

            // Assert
            Assert.AreEqual(result.Data.Item.Id, _guid);
        }

        [Test]
        public void PublishContent_Test()
        {
            // Arrange
            var content = new Mock<IPublishedContent>();
            _guid = Guid.NewGuid();
            content.Setup(x => x.IsPublished(It.IsAny<string>())).Returns(false);
            _mockFutureNhsContentService.Setup(x => x.PublishAsync(It.IsAny<Guid>()).Result).Returns(true);
            CustomContentHandlerSetup();

            // Act
            var result = _contentHandler.PublishContentAsync(_guid).Result;

            // Assert
            Assert.AreEqual(result.Succeeded, true);
        }

        [Test]
        public void DeleteContent_Test()
        {
            // Arrange 
            _mockFutureNhsContentService.Setup(x => x.DeleteAsync(It.IsAny<Guid>()).Result).Returns(true);
            CustomContentHandlerSetup();

            // Act  
            var result = _contentHandler.DeleteContentAsync(_guid).Result;

            // Assert
            Assert.AreEqual(result.Succeeded, true);
        }

        [Test]
        public void GetAllContent_Test()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _mockFutureNhsContentService.Setup(x => x.GetPublishedChildrenAsync(It.IsAny<Guid>()).Result).Returns(GenerateContentModels());

            _mockFutureNhsContentService.SetupSequence(x => x.ResolveAsync(It.IsAny<IPublishedContent>()).Result)
                .Returns(new ContentModel() { Item = new ItemModel() { Id = _guidList[0] } })
                .Returns(new ContentModel() { Item = new ItemModel() { Id = _guidList[1] } })
                .Returns(new ContentModel() { Item = new ItemModel() { Id = _guidList[2] } })
                .Returns(new ContentModel() { Item = new ItemModel() { Id = _guidList[3] } });

            CustomContentHandlerSetup();


            // Act
            var result = _contentHandler.GetAllContentAsync().Result;

            // Assert
            Assert.AreEqual(_guidList, result.Data.Select(x => x.Item.Id));
        }
    }
}