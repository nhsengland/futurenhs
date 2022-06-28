namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Resolvers.Interfaces;

    public sealed class FutureNhsContentService : IFutureNhsContentService
    {
        private readonly Lazy<IFutureNhsContentResolver> _contentResolver;
        private readonly IPublishedContentQuery _publishedContent;
        private readonly IContentService _contentService;
        private readonly ILogger<FutureNhsContentService> _logger;

        public FutureNhsContentService(IPublishedContentQuery publishedContent, Lazy<IFutureNhsContentResolver> contentResolver, IContentService contentService, ILogger<FutureNhsContentService> logger)
        {
            _publishedContent = publishedContent ?? throw new ArgumentNullException(nameof(publishedContent));
            _contentResolver = contentResolver ?? throw new ArgumentNullException(nameof(contentResolver));
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IPublishedContent GetPublishedContent(Guid contentId, CancellationToken cancellationToken)
        {
            var publishedContent = _publishedContent.Content(contentId);
            if (publishedContent is null)
            {
                _logger.LogError("Unable to get published content {ContentId}. Content does not exist.", contentId);
                throw new KeyNotFoundException($"Unable to get published content {contentId}. Content does not exist.");
            }
            return publishedContent;
        }

        /// <inheritdoc />
        public IContent GetDraftContent(Guid contentId, CancellationToken cancellationToken)
        {
            var draftContent = _contentService.GetById(contentId);
            if (draftContent is null)
            {
                _logger.LogError("Unable to get draft content {ContentId}. Content does not exist.", contentId);
                throw new KeyNotFoundException($"Unable to get draft content {contentId}. Content does not exist.");
            }
            return draftContent;
        }

        /// <inheritdoc />
        public ContentModelData ResolvePublishedContent(IPublishedContent publishedContent, string propertyGroupAlias = "content")
        {
            if (publishedContent is null)
            {
                _logger.LogError("Unable to resolve content. Content does not exist.");
                throw new KeyNotFoundException($"Unable to resolve content. Content does not exist.");
            }

            return _contentResolver.Value.ResolveContent(publishedContent, propertyGroupAlias);
        }

        /// <inheritdoc />
        public ContentModelData ResolveDraftContent(IContent content)
        {
            if (content is null)
            {
                _logger.LogError("Unable to resolve content. Content does not exist.");
                throw new KeyNotFoundException($"Unable to resolve content. Content does not exist.");
            }

            return _contentResolver.Value.ResolveContent(content);
        }

        /// <inheritdoc />
        public IContent CreateContentFromTemplate(string name, Guid parentId, Guid templateId, CancellationToken cancellationToken)
        {
            var parent = _contentService.GetById(parentId);
            var template = _contentService.GetById(templateId);
            var clonedContent = _contentService.Copy(template, parent.Id, false);
            clonedContent.Name = name;
            PublishContentWithDescendants(clonedContent, cancellationToken);
            return clonedContent;
        }

        /// <inheritdoc />
        public IContent CreateContent(string name, Guid parentId, string documentTypeAlias, CancellationToken cancellationToken)
        {
            var parentContent = _contentService.GetById(parentId);
            return _contentService.CreateAndSave(name, parentContent, documentTypeAlias);
        }

        /// <inheritdoc />
        public void DeleteContent(Guid contentId, CancellationToken cancellationToken)
        {
            var content = _contentService.GetById(contentId);
            if (content is not null)
            {
                if (!_contentService.Delete(content).Success)
                {
                    _logger.LogWarning("Unable to delete content {ContentId}. Content does not exist.", contentId);
                }
            }
        }

        /// <inheritdoc />
        public void DeleteContent(IEnumerable<Guid> contentIds, CancellationToken cancellationToken)
        {
            foreach (var block in contentIds)
            {
                var publishedBlock = GetPublishedContent(block, cancellationToken);

                // Delete child blocks
                foreach (var child in publishedBlock.Children
                    .Where(x => x.ContentType.Alias is not GeneralWebPage.ModelTypeAlias))
                {
                    DeleteContent(child.Key, cancellationToken);
                }

                // Delete block.
                DeleteContent(block, cancellationToken);
            }
        }

        /// <inheritdoc />
        public void PublishContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.SaveAndPublish(content).Success)
            {
                _logger.LogError("Unable to save and publish content {ContentId}. Content does not exist.", content.Key);
                throw new KeyNotFoundException($"Unable to save and publish content {content.Key}. Content does not exist.");
            }
        }

        /// <inheritdoc />
        public void PublishContentWithDescendants(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.SaveAndPublishBranch(content, true).All(x => x.Success))
            {
                _logger.LogError("Unable to save and publish content {ContentId} or it's descendants.", content.Key);
                throw new KeyNotFoundException($"Unable to save and publish content {content.Key} or it's descendants.");
            }
        }

        /// <inheritdoc />
        public void SaveContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.Save(content).Success)
            {
                _logger.LogError("Unable to save content {ContentId}. Content does not exist.", content.Key);
                throw new KeyNotFoundException($"Unable to save content {content.Key}. Content does not exist.");
            }
        }

        /// <inheritdoc />
        public void RollbackDraftContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.Rollback(content.Id, content.PublishedVersionId).Success)
            {
                _logger.LogError("Unable to rollback content {ContentId}. Content does not exist.", content.Key);
                throw new KeyNotFoundException($"Unable to rollback content {content.Key}. Content does not exist.");
            }
        }

        /// <inheritdoc />
        public IContent SetContentPropertyValue(IContent content, string propertyAlias, object value, CancellationToken cancellationToken)
        {
            try
            {
                content.Properties[propertyAlias].SetValue(value);
                return (content);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, "An exception occured when setting {PropertyAlias} value for {ContentId}, see the inner exception for details.", propertyAlias, content.Key);
                throw new ArgumentOutOfRangeException($"An exception occured when setting {propertyAlias} value for {content.Key}, see the inner exception for details", ex);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Guid> CompareContentModelLists(IEnumerable<ContentModelData> newContentModelList, IEnumerable<ContentModelData> currentContentModelList)
        {
            try
            {
                return newContentModelList
                        .Where(x => x.Item != null)
                        .Select(x => x.Item.Id)
                        .Where(ncml => !currentContentModelList
                            .Select(ccml => ccml.Item.Id)
                            .Contains(ncml))
                        .ToList();
            }
            catch (StackOverflowException ex)
            {
                _logger.LogError(ex, "An exception occured while comparing draft blocks against published blocks, see inner exception for details");
                throw new ArgumentOutOfRangeException("An exception occured while comparing draft blocks against published blocks, see inner exception for details");
            }
        }
    }
}
