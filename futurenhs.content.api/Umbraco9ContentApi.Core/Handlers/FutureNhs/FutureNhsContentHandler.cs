namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Services.FutureNhs.Interface;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    /// <summary>
    /// The handler that handles content methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsContentHandler" />
    public sealed class FutureNhsContentHandler : IFutureNhsContentHandler
    {
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsBlockService _futureNhsBlockService;

        public FutureNhsContentHandler(IFutureNhsContentService futureNhsContentService, IFutureNhsBlockService futureNhsBlockService)
        {
            _futureNhsContentService = futureNhsContentService ?? throw new ArgumentNullException(nameof(futureNhsContentService));
            _futureNhsBlockService = futureNhsBlockService;
        }

        /// <inheritdoc />
        public ApiResponse<string> PublishContentAndAssociatedContent(Guid contentId, CancellationToken cancellationToken)
        {
            var draftContent = _futureNhsContentService.GetDraftContent(contentId, cancellationToken);
            var publishedContent = _futureNhsContentService.GetPublishedContent(contentId, cancellationToken);

            if (draftContent is not null)
            {
                var resolvedDraftContent = _futureNhsContentService.ResolveDraftContent(draftContent, cancellationToken);
                var resolvedPublishedContent = _futureNhsContentService.ResolvePublishedContent(publishedContent, "content", cancellationToken);

                var draftBlocks = resolvedDraftContent.Content.Where(x => x.Key == "blocks").Select(c => (IEnumerable<ContentModel>)c.Value).FirstOrDefault();
                var publishedBlocks = resolvedPublishedContent.Content.Where(x => x.Key == "blocks").Select(c => (IEnumerable<ContentModel>)c.Value).FirstOrDefault();


                if (publishedBlocks is not null && draftBlocks is not null)
                {
                    // Find the difference between published and draft contents list of blocks
                    var blocksToRemove = _futureNhsContentService.CompareContentModelLists(publishedBlocks, draftBlocks);

                    foreach (var block in blocksToRemove)
                    {
                        var publishedBlock = _futureNhsContentService.GetPublishedContent(block, cancellationToken);

                        // Delete associated blocks
                        var publishedContentBlocks = _futureNhsContentService.GetAssociatedPublishedContentBlocks(publishedBlock, cancellationToken);

                        for (int i = 0; i < publishedContentBlocks.Count; i++)
                        {
                            _futureNhsContentService.DeleteContent(publishedContentBlocks[i].Key, cancellationToken);
                        }

                        // Delete block
                        _futureNhsContentService.DeleteContent(block, cancellationToken);
                    }

                    // Publish draft block child blocks
                    var draftBlocksChildBlocks = _futureNhsBlockService.GetChildBlocks(draftBlocks, cancellationToken);

                    foreach (var block in draftBlocksChildBlocks)
                    {
                        var draftBlock = _futureNhsContentService.GetDraftContent(block.Item.Id, cancellationToken);
                        _futureNhsContentService.PublishContent(draftBlock, cancellationToken);
                    }

                    // Publish draft blocks
                    foreach (var block in draftBlocks)
                    {
                        var draftBlock = _futureNhsContentService.GetDraftContent(block.Item.Id, cancellationToken);
                        _futureNhsContentService.PublishContent(draftBlock, cancellationToken);
                    }
                }
            }

            _futureNhsContentService.PublishContent(draftContent, cancellationToken);

            return new ApiResponse<string>().Success(contentId.ToString(), "Content and associated content successfully published.");
        }

        /// <inheritdoc />
        public ApiResponse<ContentModel> GetPublishedContent(Guid contentId, CancellationToken cancellationToken)
        {
            var publishedContent = _futureNhsContentService.GetPublishedContent(contentId, cancellationToken);
            return new ApiResponse<ContentModel>().Success(_futureNhsContentService.ResolvePublishedContent(publishedContent, "content", cancellationToken), "Published content found.");
        }

        /// <inheritdoc />
        public ApiResponse<ContentModel> GetDraftContent(Guid contentId, CancellationToken cancellationToken)
        {
            var publishedContent = _futureNhsContentService.GetDraftContent(contentId, cancellationToken);
            return new ApiResponse<ContentModel>().Success(_futureNhsContentService.ResolveDraftContent(publishedContent, cancellationToken), "Draft content found.");
        }

        /// <inheritdoc />
        public ApiResponse<string> DeleteContent(Guid contentId, CancellationToken cancellationToken)
        {
            _futureNhsContentService.DeleteContent(contentId, cancellationToken);
            return new ApiResponse<string>().Success(contentId.ToString(), "Content deleted successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<string> DiscardDraftContent(Guid contentId, CancellationToken cancellationToken)
        {
            var draftContent = _futureNhsContentService.GetDraftContent(contentId, cancellationToken);

            if (draftContent.ContentType.Alias == GeneralWebPage.ModelTypeAlias)
            {
                var publishedContent = _futureNhsContentService.GetPublishedContent(contentId, cancellationToken);
                var resolvedDraftContent = _futureNhsContentService.ResolveDraftContent(draftContent, cancellationToken);
                var resolvedPublishedContent = _futureNhsContentService.ResolvePublishedContent(publishedContent, "content", cancellationToken);

                // Get context content blocks
                var draftBlocks = resolvedDraftContent.Content.Where(x => x.Key == "blocks").Select(c => c.Value).FirstOrDefault() as IEnumerable<ContentModel>;
                var publishedBlocks = resolvedPublishedContent.Content.Where(x => x.Key == "blocks").Select(c => c.Value).FirstOrDefault() as IEnumerable<ContentModel>;

                // For each block rollback to published version
                if (publishedBlocks is not null && draftBlocks is not null)
                {
                    foreach (var block in draftBlocks)
                    {
                        var draft = _futureNhsContentService.GetDraftContent(block.Item.Id, cancellationToken);
                        _futureNhsContentService.RollbackDraftContent(draft, cancellationToken);
                    }

                    // Find the difference between draft and published contents list of blocks.
                    var blocksToRemove = _futureNhsContentService.CompareContentModelLists(draftBlocks, publishedBlocks);

                    foreach (var block in blocksToRemove)
                    {
                        var publishedBlock = _futureNhsContentService.GetPublishedContent(block, cancellationToken);

                        // Delete associated blocks.
                        var publishedContentBlocks = _futureNhsContentService.GetAssociatedPublishedContentBlocks(publishedBlock, cancellationToken);

                        for (int i = 0; i < publishedContentBlocks.Count; i++)
                        {
                            _futureNhsContentService.DeleteContent(publishedContentBlocks[i].Key, cancellationToken);
                        }

                        // Delete block.
                        _futureNhsContentService.DeleteContent(block, cancellationToken);
                    }
                }

                // Rollback context content to published version.
                _futureNhsContentService.RollbackDraftContent(draftContent, cancellationToken);

                // Publish to reset draft status to false. 
                _futureNhsContentService.PublishContent(draftContent, cancellationToken);
            }

            return new ApiResponse<string>().Success(contentId.ToString(), "Content discarded successfully.");
        }
    }
}
