namespace Umbraco9ContentApi.Test.Services
{
    using Core.Services.FutureNhs.Interface;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.PublishedCache;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using Umbraco9ContentApi.Core.Resolvers.Interfaces;
    using Umbraco9ContentApi.Core.Services.FutureNhs;
    using ContentModelData = Core.Models.Content.ContentModelData;

    /// <summary>
    /// Futrue Nhs Content Handler Tests.
    /// </summary>
    [TestFixture]
    public class FutureNhsContentServiceTests
    {
        // Mock Services
        private Mock<IFutureNhsContentService> _mockFutureNhsContentService = new();
        private Mock<IPublishedContentQuery> _mockPublishedContentQuery = new();
        private Mock<Lazy<IFutureNhsContentResolver>> _mockFutureNhsContentResolver = new();
        private Mock<IContentService> _mockContentService = new();
        private Mock<IPublishedSnapshotAccessor> _mockPublishedSnapshotAccessor = new();

        private FutureNhsContentService _futureNhsContentService;

        private IConfiguration _config;
        private CancellationToken cancellationToken;

        private static ContentModelData contentModelTextBlock = new();
        private static ContentModelData contentModelKeyLinksBlock = new();
        private static List<ContentModelData> contentModelList = new();

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> { { "AppKeys:Folders:Groups", Guid.NewGuid().ToString() } };
            _config = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();

            _mockFutureNhsContentService = new Mock<IFutureNhsContentService>().SetupAllProperties();
            _mockPublishedContentQuery = new Mock<IPublishedContentQuery>().SetupAllProperties(); ;
            _mockFutureNhsContentResolver = new Mock<Lazy<IFutureNhsContentResolver>>().SetupAllProperties();
            _mockContentService = new Mock<IContentService>().SetupAllProperties();
            _mockPublishedSnapshotAccessor = new Mock<IPublishedSnapshotAccessor>().SetupAllProperties();

            contentModelTextBlock = GetMockBlockContentModel(TextBlock.ModelTypeAlias).Object;
            contentModelKeyLinksBlock = GetMockBlockContentModel(KeyLinksBlock.ModelTypeAlias).Object;
        }

        public void GetFutureNhsContentService()
        {
            _futureNhsContentService = new(_mockPublishedContentQuery.Object,
_mockFutureNhsContentResolver.Object, _mockContentService.Object);
        }


        [Test]
        public void GetPublishedContent_ContentExists_Success()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockPublishedContent = GetMockPublishedContentItem(contentId, true);
            _mockPublishedContentQuery.Setup(x => x.Content(It.IsAny<Guid>())).Returns(mockPublishedContent.Object);

            var expectedResult = mockPublishedContent.Object;

            GetFutureNhsContentService();

            //Action
            var result = _futureNhsContentService.GetPublishedContent(contentId, cancellationToken);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult.Key, result.Key);
        }

        [Test]
        public void GetPublishedContent_ContentDoesNotExists_ThrowsException()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockPublishedContent = GetMockPublishedContentItem(contentId, true);
            _mockPublishedContentQuery.Setup(x => x.Content(It.IsAny<Guid>())).Returns((IPublishedContent)null);
            var message = $"Unable to get published content {contentId}. Content does not exist.";

            var expectedResult = mockPublishedContent.Object;

            GetFutureNhsContentService();

            //Action/Assert
            Assert.Throws(Is.TypeOf<KeyNotFoundException>()
                 .And.Message.EqualTo(message), delegate { _futureNhsContentService.GetPublishedContent(contentId, cancellationToken); });
        }

        [Test]
        public void GetDraftContent_ContentExists_Success()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(mockDraftContent.Object);

            var expectedResult = mockDraftContent.Object;

            GetFutureNhsContentService();

            //Action
            var result = _futureNhsContentService.GetDraftContent(contentId, cancellationToken);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult.Key, result.Key);
        }

        [Test]
        public void GetDraftContent_ContentDoesNotExists_ThrowsException()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns((IContent)null);
            var message = $"Unable to get draft content {contentId}. Content does not exist.";

            var expectedResult = mockDraftContent.Object;

            GetFutureNhsContentService();

            //Action/Assert
            Assert.Throws(Is.TypeOf<KeyNotFoundException>()
                 .And.Message.EqualTo(message), delegate { _futureNhsContentService.GetDraftContent(contentId, cancellationToken); });
        }

        [Test]
        public void DeleteContent_ContentExists_Success()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.Delete(It.IsAny<IContent>(), It.IsAny<int>())).Returns(new OperationResult(OperationResultType.Success, null));

            var expectedResult = mockDraftContent.Object;

            GetFutureNhsContentService();

            //Action/Assert
            Assert.DoesNotThrow(() => _futureNhsContentService.DeleteContent(contentId, cancellationToken));
        }

        [Test]
        public void DeleteContent_ContentDeleteError_ThrowsException()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.Delete(It.IsAny<IContent>(), It.IsAny<int>())).Returns(new OperationResult(OperationResultType.Failed, null));
            var message = $"Unable to delete content {contentId}. Content does not exist.";

            var expectedResult = mockDraftContent.Object;

            GetFutureNhsContentService();

            //Action/Assert
            Assert.Throws(Is.TypeOf<KeyNotFoundException>()
                 .And.Message.EqualTo(message), delegate { _futureNhsContentService.DeleteContent(contentId, cancellationToken); });
        }

        [Test]
        public void SaveContent_ContentExists_Success()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.Save(It.IsAny<IContent>(), It.IsAny<int>())).Returns(new OperationResult(OperationResultType.Success, null));

            var expectedResult = new OperationResult(OperationResultType.Success, null);

            GetFutureNhsContentService();

            //Action/Assert
            Assert.DoesNotThrow(() => _futureNhsContentService.SaveContent(mockDraftContent.Object, cancellationToken));
        }

        [Test]
        public void SaveContent_ContentDoesNotExists_ThrowsException()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.Save(It.IsAny<IContent>(), It.IsAny<int>())).Returns(new OperationResult(OperationResultType.Failed, null));
            var message = $"Unable to save content {mockDraftContent.Object.Key}. Content does not exist.";

            var expectedResult = mockDraftContent.Object;

            GetFutureNhsContentService();

            //Action/Assert
            Assert.Throws(Is.TypeOf<KeyNotFoundException>()
                 .And.Message.EqualTo(message), delegate { _futureNhsContentService.SaveContent(mockDraftContent.Object, cancellationToken); });
        }

        [Test]
        public void SaveAndPublishContent_ContentExists_Success()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.SaveAndPublish(It.IsAny<IContent>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new PublishResult(PublishResultType.SuccessPublish, null, mockDraftContent.Object));

            GetFutureNhsContentService();

            //Action/Assert
            Assert.DoesNotThrow(() => _futureNhsContentService.PublishContent(mockDraftContent.Object, cancellationToken));
        }

        [Test]
        public void SaveAndPublishContent_ContentDoesNotExists_ThrowsException()
        {
            //Arrange
            var contentId = Guid.NewGuid();
            var mockDraftContent = GetMockContent(contentId);
            _mockContentService.Setup(x => x.SaveAndPublish(It.IsAny<IContent>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new PublishResult(PublishResultType.FailedPublish, null, mockDraftContent.Object));
            var message = $"Unable to save and publish content {mockDraftContent.Object.Key}. Content does not exist.";

            var expectedResult = mockDraftContent.Object;

            GetFutureNhsContentService();

            //Action/Assert
            Assert.Throws(Is.TypeOf<KeyNotFoundException>()
                 .And.Message.EqualTo(message), delegate { _futureNhsContentService.PublishContent(mockDraftContent.Object, cancellationToken); });
        }



        [Test]
        public void CompareContentModelLists_WithDifference_Success()
        {
            // Arrange          
            Mock<List<ContentModelData>> originalContentModelList = GetMockContentModelList();
            Mock<List<ContentModelData>> contentModelList = new();
            Mock<List<ContentModelData>> comparedContentModelList = new();
            contentModelList.Object.AddRange(originalContentModelList.Object);
            comparedContentModelList.Object.AddRange(originalContentModelList.Object);

            List<Guid> expectedResult = new();

            comparedContentModelList.Object.Remove(comparedContentModelList.Object
                .Where(x => x.Item.ContentType == KeyLinksBlock.ModelTypeAlias).FirstOrDefault());

            expectedResult.Add(contentModelList.Object
                .Where(x => x.Item.ContentType == KeyLinksBlock.ModelTypeAlias).FirstOrDefault().Item.Id);

            GetFutureNhsContentService();

            // Act
            var contentResult = _futureNhsContentService.CompareContentModelLists(contentModelList.Object, comparedContentModelList.Object);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(expectedResult, contentResult);
        }

        [Test]
        public void CompareContentModelLists_NoDifference_Success()
        {
            // Arrange          
            Mock<List<ContentModelData>> originalContentModelList = GetMockContentModelList();
            Mock<List<ContentModelData>> contentModelList = new();
            Mock<List<ContentModelData>> comparedContentModelList = new();
            contentModelList.Object.AddRange(originalContentModelList.Object);
            comparedContentModelList.Object.AddRange(originalContentModelList.Object);

            List<Guid> expectedResult = new();

            GetFutureNhsContentService();

            // Act
            var contentResult = _futureNhsContentService.CompareContentModelLists(contentModelList.Object, comparedContentModelList.Object);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(expectedResult, contentResult);
        }

        #region Setup

        /// <summary>
        /// Gets the mock content model.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        private Mock<ContentModelData> GetMockBlockContentModel(string contentType)
        {
            var dict = new Dictionary<string, object>()
            {
                { "Field", "Value" },
            };

            var mockContentModel = new Mock<ContentModelData>();
            mockContentModel.Setup(x => x.Item.Id).Returns(Guid.NewGuid());
            mockContentModel.Setup(x => x.Item.ContentType).Returns(contentType);
            mockContentModel.Setup(x => x.Content).Returns(dict);
            return mockContentModel;
        }

        /// <summary>
        /// Gets the mock content item.
        /// </summary>
        /// <returns></returns>
        private Mock<IContent> GetMockContent(Guid contentId)
        {
            var mockContent = new Mock<IContent>();
            mockContent.SetupAllProperties();
            mockContent.Setup(x => x.Key).Returns(contentId);
            mockContent.Setup(x => x.Properties.GetEnumerator()).Returns(GetPropertyCollection("blocks", "mainText").GetEnumerator());
            mockContent.Setup(x => x.ContentType.Alias).Returns(GeneralWebPage.ModelTypeAlias);
            return mockContent;
        }

        /// <summary>
        /// Gets the mock published content item.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="isPublished">if set to <c>true</c> [is published].</param>
        /// <returns></returns>
        private Mock<IPublishedContent> GetMockPublishedContentItem(Guid contentId, bool isPublished)
        {
            var mockContent = new Mock<IPublishedContent>();
            mockContent.Setup(x => x.Key).Returns(contentId);
            mockContent.Setup(x => x.IsPublished(It.IsAny<string>())).Returns(isPublished);
            return mockContent;
        }

        /// <summary>
        /// Gets the mock property.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        private IPropertyCollection GetPropertyCollection(params string[] alias)
        {
            List<IProperty> propertyList = new();
            Mock<IPropertyCollection> mockPropertyCollection = new();

            for (int i = 0; i < alias.Length; i++)
            {
                var mockProperty = new Mock<IProperty>();
                mockProperty.Setup(x => x.Alias).Returns(alias[i]);
                propertyList.Add(mockProperty.Object);
            }

            mockPropertyCollection.Setup(x => x.GetEnumerator()).Returns(propertyList.GetEnumerator());
            return mockPropertyCollection.Object;
        }

        private Mock<List<ContentModelData>> GetMockContentModelList()
        {
            Mock<List<ContentModelData>> contentModelList = new();

            Mock<ContentModelData> block1 = GetMockBlockContentModel(TextBlock.ModelTypeAlias);
            Mock<ContentModelData> block2 = GetMockBlockContentModel(TextBlock.ModelTypeAlias);
            Mock<ContentModelData> block3 = GetMockBlockContentModel(KeyLinksBlock.ModelTypeAlias);

            contentModelList.Object.Add(block1.Object);
            contentModelList.Object.Add(block2.Object);
            contentModelList.Object.Add(block3.Object);

            return contentModelList;
        }



        #endregion
    }
}
