namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using Interface;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
    using UmbracoContentApi.Core.Resolvers;
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;

    /// <inheritdoc />
    public class FutureNhsContentService : IFutureNhsContentService
    {
        private readonly Lazy<IContentResolver> _contentResolver;
        private readonly IPublishedContentQuery _publishedContent;
        private readonly IContentService _contentService;

        /// <summary>Initializes a new instance of the <see cref="FutureNhsContentService" /> class.</summary>
        /// <param name="publishedContent">Content of the published.</param>
        /// <param name="contentResolver">The content resolver.</param>
        /// <param name="contentService">The content service.</param>
        public FutureNhsContentService(IPublishedContentQuery publishedContent, Lazy<IContentResolver> contentResolver, IContentService contentService)
        {
            _publishedContent = publishedContent;
            _contentResolver = contentResolver;
            _contentService = contentService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IPublishedContent>> GetPublishedChildren(Guid id)
        {
            var children = _publishedContent.Content(id).Children;
            return children != null && children.Any() ? children : new List<IPublishedContent>();
        }

        /// <inheritdoc />
        public async Task<IPublishedContent> GetPublished(Guid id)
        {
            return _publishedContent.Content(id);
        }

        /// <inheritdoc />
        public async Task<IContent> Get(Guid id)
        {
            return _contentService.GetById(id);
        }

        /// <inheritdoc />
        public async Task<ContentModel> Resolve(IPublishedContent content)
        {
            return _contentResolver.Value.ResolveContent(content);
        }

        /// <inheritdoc />
        public async Task<IContent?> Create(Guid parentContentId, string contentName, string documentTypeAlias)
        {
            var parentContent = _contentService.GetById(parentContentId);
            var result = _contentService.CreateAndSave(contentName, parentContent, documentTypeAlias);
            return result;
        }

        /// <inheritdoc />
        public async Task<bool> Delete(Guid id)
        {
            var content = _contentService.GetById(id);

            if (content == null)
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
        public async Task<bool> Publish(Guid id)
        {
            var content = _contentService.GetById(id);
            var result = _contentService.SaveAndPublish(content);
            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ContentModel>> GetTemplateBlocks(Guid id)
        {
            var contentModels = new List<ContentModel>();
            var template = _publishedContent.Content(id);
            var blocks = template.GetProperty("blockPicker")
                .GetValue() as List<IPublishedContent>;

            if (blocks == null || !blocks.Any())
            {
                return new List<ContentModel>();
            }

            foreach (var block in blocks)
            {
                contentModels.Add(await Resolve(block));
            }

            return contentModels != null ? contentModels : new List<ContentModel>();
        }

        /// <inheritdoc />
        public async Task<bool> Save(IContent content)
        {
            var result = _contentService.Save(content);
            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<bool> SaveAndPublish(IContent content)
        {
            var result = _contentService.SaveAndPublish(content);
            return true ? result.Success : false;
        }
    }
}
