namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using Interface;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
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
        public async Task<IEnumerable<IPublishedContent>> GetPublishedChildrenAsync(Guid id)
        {
            return _publishedContent.Content(id).Children ?? null;
        }

        /// <inheritdoc />
        public async Task<IPublishedContent> GetPublishedAsync(Guid id)
        {
            return _publishedContent.Content(id);
        }

        /// <inheritdoc />
        public async Task<IContent> GetAsync(Guid id)
        {
            return _contentService.GetById(id);
        }

        /// <inheritdoc />
        public async Task<ContentModel> ResolveAsync(IPublishedContent content)
        {
            return _contentResolver.Value.ResolveContent(content);
        }

        /// <inheritdoc />
        public async Task<IContent?> CreateAsync(Guid parentContentId, string contentName, string documentTypeAlias)
        {
            var parentContent = _contentService.GetById(parentContentId);
            var result = _contentService.CreateAndSave(contentName, parentContent, documentTypeAlias);
            return result;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid id)
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
        public async Task<bool> PublishAsync(Guid contentId)
        {
            var content = _contentService.GetById(contentId);
            var result = _contentService.SaveAndPublish(content);
            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ContentModel>> GetTemplateBlocksAsync(Guid id)
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
                contentModels.Add(await ResolveAsync(block));
            }

            return contentModels is not null ? contentModels : new List<ContentModel>();
        }

        /// <inheritdoc />
        public async Task<bool> SaveAsync(IContent content)
        {
            var result = _contentService.Save(content);
            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<bool> SaveAndPublishAsync(IContent content)
        {
            var result = _contentService.SaveAndPublish(content);
            return true ? result.Success : false;
        }
    }
}
