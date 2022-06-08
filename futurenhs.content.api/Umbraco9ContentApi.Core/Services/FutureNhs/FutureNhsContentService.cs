namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using Interface;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
    using Umbraco9ContentApi.Core.Resolvers.Interfaces;
    using static Umbraco.Cms.Core.Constants;
    using ContentModelData = Models.Content.ContentModelData;

    /// <inheritdoc />
    public sealed class FutureNhsContentService : IFutureNhsContentService
    {
        private readonly Lazy<IFutureNhsContentResolver> _contentResolver;
        private readonly IPublishedContentQuery _publishedContent;
        private readonly IContentService _contentService;

        public FutureNhsContentService(IPublishedContentQuery publishedContent, Lazy<IFutureNhsContentResolver> contentResolver, IContentService contentService)
        {
            _publishedContent = publishedContent ?? throw new ArgumentNullException(nameof(publishedContent));
            _contentResolver = contentResolver ?? throw new ArgumentNullException(nameof(contentResolver));
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
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
        public ContentModelData ResolvePublishedContent(IPublishedContent publishedContent, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            return _contentResolver.Value.ResolveContent(publishedContent, propertyGroupAlias);
        }

        /// <inheritdoc />
        public ContentModelData ResolveDraftContent(IContent content, CancellationToken cancellationToken)
        {
            return _contentResolver.Value.ResolveContent(content);
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

            if (!_contentService.Delete(content).Success)
                throw new KeyNotFoundException($"Unable to delete content {contentId}. Content does not exist.");
        }

        /// <inheritdoc />
        public void PublishContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.SaveAndPublish(content).Success)
                throw new KeyNotFoundException($"Unable to save and publish content {content.Key}. Content does not exist.");
        }

        /// <inheritdoc />
        public List<ContentModelData> GetTemplateBlocks(Guid templateId, CancellationToken cancellationToken)
        {
            var contentModelList = new List<ContentModelData>();

            var template = _publishedContent.Content(templateId);

            if (template is null)
                throw new KeyNotFoundException($"Unable to get template {templateId}. Template does not exist.");

            var blocks = template.GetProperty("blockPicker")
                .GetValue() as List<IPublishedContent>;

            if (blocks is not null || blocks.Any())
            {
                foreach (var block in blocks)
                {
                    contentModelList.Add(ResolvePublishedContent(block, "content", cancellationToken));
                }
            }

            return contentModelList;
        }

        /// <inheritdoc />
        public void SaveContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.Save(content).Success)
                throw new KeyNotFoundException($"Unable to save content {content.Key}. Content does not exist.");
        }

        /// <inheritdoc />
        public void SaveAndPublishContent(IContent content, CancellationToken cancellationToken)
        {
            if (!_contentService.SaveAndPublish(content).Success)
                throw new KeyNotFoundException($"Unable to save and publish content {content.Key}. Content does not exist.");
        }

        /// <inheritdoc />
        public IContent AssignBlockToContent(IContent contentToUpdate, Guid blockId, CancellationToken cancellationToken)
        {
            List<string> contentBlocksUdiList = new();

            string blockUdi = Udi.Create(UdiEntityType.Document, blockId).ToString();

            contentToUpdate.Properties.TryGetValue("blocks", out IProperty blocksProperty);

            if (blocksProperty is not null && blocksProperty.Values.Any())
            {
                contentBlocksUdiList.AddRange(blocksProperty
                    .GetValue()
                    .ToString()
                    .Split(',')
                    .ToList());
            }

            contentBlocksUdiList.Add(blockUdi);

            return SetContentPropertyValue(contentToUpdate, "blocks", string.Join(",", contentBlocksUdiList), cancellationToken);
        }

        /// <inheritdoc />
        public List<IPublishedContent> GetAssociatedPublishedContentBlocks(IPublishedContent content, CancellationToken cancellationToken)
        {
            List<IPublishedContent> pubishedBlocksList = new();

            var publishedBlocks = (List<IPublishedContent>?)content.Properties
                .Where(p => p.Alias == "blocks")
                .Select(x => x.GetValue())
                .FirstOrDefault();

            if (publishedBlocks is not null && publishedBlocks.Any())
            {
                pubishedBlocksList.AddRange(publishedBlocks);
            }

            return pubishedBlocksList;
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
