namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using Interface;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
    using Umbraco9ContentApi.Core.Models.Dto;
    using Umbraco9ContentApi.Core.Resolvers.Interfaces;
    using ContentModel = Models.Content.ContentModel;

    /// <inheritdoc />
    public sealed class FutureNhsContentService : IFutureNhsContentService
    {
        private readonly Lazy<IFutureNhsContentResolver> _contentResolver;
        private readonly IPublishedContentQuery _publishedContent;
        private readonly IContentService _contentService;

        /// <summary>Initializes a new instance of the <see cref="FutureNhsContentService" /> class.</summary>
        /// <param name="publishedContent">Content of the published.</param>
        /// <param name="contentResolver">The content resolver.</param>
        /// <param name="contentService">The content service.</param>
        public FutureNhsContentService(IPublishedContentQuery publishedContent, Lazy<IFutureNhsContentResolver> contentResolver, IContentService contentService)
        {
            _publishedContent = publishedContent;
            _contentResolver = contentResolver;
            _contentService = contentService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IPublishedContent>> GetPublishedContentChildrenAsync(Guid id, CancellationToken cancellationToken)
        {
            return _publishedContent.Content(id).Children ?? null;
        }

        /// <inheritdoc />
        public async Task<IPublishedContent> GetPublishedContentAsync(Guid id, CancellationToken cancellationToken)
        {
            return _publishedContent.Content(id);
        }

        /// <inheritdoc />
        public async Task<IContent> GetDraftContentAsync(Guid id, CancellationToken cancellationToken)
        {
            return _contentService.GetById(id);
        }

        /// <inheritdoc />
        public async Task<ContentModel> ResolvePublishedContentAsync(IPublishedContent content, CancellationToken cancellationToken)
        {
            return _contentResolver.Value.ResolveContent(content);
        }

        /// <inheritdoc />
        public async Task<ContentModel> ResolveDraftContentAsync(IContent content, CancellationToken cancellationToken)
        {
            return _contentResolver.Value.ResolveContent(content);
        }

        /// <inheritdoc />
        public async Task<IContent?> CreateContentAsync(GeneralWebPageDto generalWebPage, CancellationToken cancellationToken)
        {
            var parentContent = _contentService.GetById(generalWebPage.PageParentId);
            var result = _contentService.CreateAndSave(generalWebPage.PageName, parentContent, generalWebPage.DocumentTypeAlias);
            return result;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteContentAsync(Guid id, CancellationToken cancellationToken)
        {
            var content = _contentService.GetById(id);

            if (content is null)
            {
                return false;
            }

            if (content.Published)
            {
                _contentService.Unpublish(content);
            }

            var result = _contentService.Delete(content);

            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<bool> PublishContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var content = _contentService.GetById(contentId);
            var result = _contentService.SaveAndPublish(content);
            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ContentModel>> GetTemplateBlocksAsync(Guid id, CancellationToken cancellationToken)
        {
            var contentModels = new List<ContentModel>();
            var template = _publishedContent.Content(id);
            var blocks = template.GetProperty("blockPicker")
                .GetValue() as List<IPublishedContent>;

            if (blocks is null || !blocks.Any())
            {
                return new List<ContentModel>();
            }

            foreach (var block in blocks)
            {
                contentModels.Add(await ResolvePublishedContentAsync(block, cancellationToken));
            }

            return contentModels is not null ? contentModels : new List<ContentModel>();
        }

        /// <inheritdoc />
        public async Task<bool> SaveContentAsync(IContent content, CancellationToken cancellationToken)
        {
            var result = _contentService.Save(content);
            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<bool> SaveAndPublishContentAsync(IContent content, CancellationToken cancellationToken)
        {
            var result = _contentService.SaveAndPublish(content);
            return true ? result.Success : false;
        }
    }
}
