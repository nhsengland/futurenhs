using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco9ContentApi.Core.Handlers.FutureNhs;
using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;
using UmbracoContentApi.Core.Models;

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
        private readonly Mock<IFutureNhsContentService> _contentService = new Mock<IFutureNhsContentService>();

        // variables
        private readonly List<IPublishedContent> _publishedContentList = new List<IPublishedContent>();
        private readonly List<Guid> _guidList = new List<Guid>();
        private Guid _guid;
        private IConfiguration _config;

        private void CustomContentHandlerSetup()
        {
            _contentHandler = new FutureNhsContentHandler(_contentService.Object, _config);
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
            _contentService.Setup(x => x.ResolveAsync(It.IsAny<IPublishedContent>()).Result).Returns(new ContentModel() { System = new SystemModel() { Id = _guid } });
            CustomContentHandlerSetup();

            // Act
            var result = _contentHandler.GetContentAsync(_guid).Result;

            // Assert
            Assert.AreEqual(result.System.Id, _guid);
        }

        [Test]
        public void PublishContent_Test()
        {
            // Arrange
            var content = new Mock<IPublishedContent>();
            _guid = Guid.NewGuid();
            content.Setup(x => x.IsPublished(It.IsAny<string>())).Returns(false);
            _contentService.Setup(x => x.PublishAsync(It.IsAny<Guid>()).Result).Returns(true);
            CustomContentHandlerSetup();

            // Act
            var result = _contentHandler.PublishContentAsync(_guid).Result;

            // Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void DeleteContent_Test()
        {
            // Arrange 
            _contentService.Setup(x => x.DeleteAsync(It.IsAny<Guid>()).Result).Returns(true);
            CustomContentHandlerSetup();

            // Act  
            var result = _contentHandler.DeleteContentAsync(_guid).Result;

            // Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void GetAllContent_Test()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _contentService.Setup(x => x.GetPublishedChildrenAsync(It.IsAny<Guid>()).Result).Returns(GenerateContentModels());

            _contentService.SetupSequence(x => x.ResolveAsync(It.IsAny<IPublishedContent>()).Result)
                .Returns(new ContentModel() { System = new SystemModel() { Id = _guidList[0] } })
                .Returns(new ContentModel() { System = new SystemModel() { Id = _guidList[1] } })
                .Returns(new ContentModel() { System = new SystemModel() { Id = _guidList[2] } })
                .Returns(new ContentModel() { System = new SystemModel() { Id = _guidList[3] } });

            CustomContentHandlerSetup();


            // Act
            var result = _contentHandler.GetAllContentAsync().Result;

            // Assert
            Assert.AreEqual(_guidList, result.Select(x => x.System.Id).ToList());
        }
    }
}