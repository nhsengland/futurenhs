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

    /// <inheritdoc />
    public sealed class FutureNhsContentService : IFutureNhsContentService
    {
        private readonly Lazy<IFutureNhsContentResolver> _contentResolver;
        private readonly IPublishedContentQuery _publishedContent;
        private readonly IContentService _contentService;
        private readonly ILogger _logger;

        public FutureNhsContentService(IPublishedContentQuery publishedContent, Lazy<IFutureNhsContentResolver> contentResolver, IContentService contentService, ILogger logger)
        {
            _publishedContent = publishedContent ?? throw new ArgumentNullException(nameof(publishedContent));
            _contentResolver = contentResolver ?? throw new ArgumentNullException(nameof(contentResolver));
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IPublishedContent GetPublishedContent(Guid contentId, CancellationToken cancellationToken)
        {
            var content = _publishedContent.Content(contentId);

            if (content is null)
                throw new KeyNotFoundException($"Unable to get published content {contentId}. Content does not exist.");

            return content;
        }

        /// <inheritdoc />
        public IContent GetDraftContent(Guid contentId, CancellationToken cancellationToken)
        {
            var content = _contentService.GetById(contentId);

            if (content is null)
                throw new KeyNotFoundException($"Unable to get draft content {contentId}. Content does not exist.");

            return content;
        }

        /// <inheritdoc />
        public ContentModelData ResolvePublishedContent(IPublishedContent publishedContent, string propertyGroupAlias = "content")
        {
            return _contentResolver.Value.ResolveContent(publishedContent, propertyGroupAlias);
        }

        /// <inheritdoc />
        public ContentModelData ResolveDraftContent(IContent content)
        {
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
                    _logger.LogWarning($"Unable to delete content {contentId}. Content does not exist.");
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
                foreach (var child in publishedBlock.Children.Where(x => x.ContentType.Alias is not GeneralWebPage.ModelTypeAlias))
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
                throw new KeyNotFoundException($"Unable to save and publish content {content.Key}. Content does not exist.");
        }

        /// <inheritdoc />
        public void PublishContentWithDescendants(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.SaveAndPublishBranch(content, true).All(x => x.Success))
                throw new KeyNotFoundException($"Unable to save and publish content {content.Key} or it's descendants.");
        }

        /// <inheritdoc />
        public void SaveContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.Save(content).Success)
                throw new KeyNotFoundException($"Unable to save content {content.Key}. Content does not exist.");
        }

        /// <inheritdoc />
        public void RollbackDraftContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.Rollback(content.Id, content.PublishedVersionId).Success)
                throw new KeyNotFoundException($"Unable to rollback content {content.Key}. Content does not exist.");
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
                throw new ArgumentOutOfRangeException($"An exception occured when setting {propertyAlias} value for {content.Key}, see the inner exception for details", ex);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Guid> CompareContentModelLists(IEnumerable<ContentModelData> contentModelList, IEnumerable<ContentModelData> comparedcontentModelList)
        {
            return contentModelList.Where(x => x.Item != null).Select(x => x.Item.Id).Where(cm => !comparedcontentModelList.Select(ccm => ccm.Item.Id).Contains(cm)).ToList();
        }
    }
}
