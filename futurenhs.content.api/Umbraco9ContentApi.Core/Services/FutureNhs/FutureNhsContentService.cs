namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using Interface;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
    using Umbraco9ContentApi.Core.Resolvers.Interfaces;
    using static Umbraco.Cms.Core.Constants;
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
        public async Task<IEnumerable<IPublishedContent>?> GetPublishedContentChildrenAsync(Guid id, CancellationToken cancellationToken)
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
        public async Task<ContentModel> ResolvePublishedContentAsync(IPublishedContent content, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            return _contentResolver.Value.ResolveContent(content, propertyGroupAlias);
        }

        /// <inheritdoc />
        public async Task<ContentModel> ResolveDraftContentAsync(IContent content, CancellationToken cancellationToken)
        {
            return _contentResolver.Value.ResolveContent(content);
        }

        /// <inheritdoc />
        public async Task<IContent?> CreateContentAsync(string name, Guid parentId, string documentTypeAlias, CancellationToken cancellationToken)
        {
            var parentContent = _contentService.GetById(parentId);
            return _contentService.CreateAndSave(name, parentContent, documentTypeAlias);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteContentAsync(Guid id, CancellationToken cancellationToken)
        {
            var content = _contentService.GetById(id);

            if (content is null)
            {
                return false;
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
                contentModels.Add(await ResolvePublishedContentAsync(block, "content", cancellationToken));
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

        /// <inheritdoc />
        public async Task<bool> AssignBlockToContent(Guid parentBlockId, Guid blockId, CancellationToken cancellationToken)
        {
            var contentToUpdate = await GetDraftContentAsync(parentBlockId, cancellationToken);

            if (contentToUpdate is null)
                return false;

            contentToUpdate.Properties.TryGetValue("blocks", out IProperty contentBlocks);

            var udiList = contentBlocks is not null
                && contentBlocks.Values.Any() ? contentBlocks
                 .GetValue()?
                 .ToString()?
                 .Split(',')
                 .ToList() : new List<string>();

            string blockUdi = Udi.Create(UdiEntityType.Document, blockId).ToString();
            udiList.Add(blockUdi);
            contentToUpdate.Properties[$"blocks"].SetValue(string.Join(",", udiList));
            var result = _contentService.Save(contentToUpdate);

            return result.Success;
        }

        /// <inheritdoc />
        public List<IPublishedContent>? GetAssociatedPublishedBlocks(IPublishedContent content)
        {
            if (content is not null)
            {
                return content.Properties.Where(p => p.Alias == "blocks").Select(x => x.GetValue())
                             .FirstOrDefault() as List<IPublishedContent>;
            }

            return null;
        }

        /// <inheritdoc />
        public bool RollbackDraftContentAsync(IContent draftContent, CancellationToken cancellationToken)
        {
            var result = _contentService.Rollback(draftContent.Id, draftContent.PublishedVersionId);
            return true ? result.Success : false;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAssociatedContent(Guid blockId, CancellationToken cancellationToken)
        {
            bool result = true;
            var block = await GetPublishedContentAsync(blockId, cancellationToken);
            var associatedBlocks = GetAssociatedPublishedBlocks(block);

            if (associatedBlocks is not null && associatedBlocks.Any())
            {
                foreach (var associatedBlock in associatedBlocks)
                {
                    result = await DeleteContentAsync(associatedBlock.Key, cancellationToken);

                    if (!result)
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            // TODO: add checks for valid page and error handling
            var page = await GetDraftContentAsync(pageId, cancellationToken);
            page.Properties["userEditing"].SetValue(userId.ToString());
            return await SaveContentAsync(page, cancellationToken);
        }
    }
}
