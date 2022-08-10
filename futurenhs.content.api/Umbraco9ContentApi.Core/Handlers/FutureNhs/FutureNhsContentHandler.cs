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

            var resolvedDraftContent = _futureNhsContentService.ResolveDraftContent(draftContent);
            var resolvedPublishedContent = _futureNhsContentService.ResolvePublishedContent(publishedContent);

            // Get context content blocks
            var draftBlocks = publishedContent.Children.Select(x => _futureNhsContentService.ResolveDraftContent(_futureNhsContentService.GetDraftContent(x.Key, cancellationToken)));
            var publishedBlocks = publishedContent.Children.Select(x => _futureNhsContentService.ResolvePublishedContent(_futureNhsContentService.GetPublishedContent(x.Key, cancellationToken)));

            // Find the difference between published and draft contents list of blocks
            var blocksToRemove = _futureNhsContentService.CompareContentModelLists(publishedBlocks, draftBlocks);

            // Delete blocks.
            _futureNhsContentService.DeleteContent(blocksToRemove, cancellationToken);

            // Publish draft blocks
            foreach (var block in draftBlocks)
            {
                var draftBlock = _futureNhsContentService.GetDraftContent(block.Item.Id, cancellationToken);
                _futureNhsContentService.PublishContent(draftBlock, cancellationToken);
            }

            // Publish draft blocks child blocks
            var draftBlocksChildBlocks = _futureNhsBlockService.GetAllDescendentBlockIds(draftBlocks, cancellationToken);

            foreach (var block in draftBlocksChildBlocks)
            {
                var draftBlock = _futureNhsContentService.GetDraftContent(block.Item.Id, cancellationToken);
                _futureNhsContentService.PublishContent(draftBlock, cancellationToken);
            }

            // Publish main content
            _futureNhsContentService.PublishContent(draftContent, cancellationToken);

            return new ApiResponse<string>().Success(contentId.ToString(), "Content and associated content successfully published.");
        }

        /// <inheritdoc />
        public ApiResponse<ContentModelData> GetPublishedContent(Guid contentId, CancellationToken cancellationToken)
        {
            var publishedContent = _futureNhsContentService.GetPublishedContent(contentId, cancellationToken);
            return new ApiResponse<ContentModelData>().Success(_futureNhsContentService.ResolvePublishedContent(publishedContent), "Published content found.");
        }

        /// <inheritdoc />
        public ApiResponse<ContentModelData> GetDraftContent(Guid contentId, CancellationToken cancellationToken)
        {
            var draftContent = _futureNhsContentService.GetDraftContent(contentId, cancellationToken);
            return new ApiResponse<ContentModelData>().Success(_futureNhsContentService.ResolveDraftContent(draftContent), "Draft content found.");
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
                var resolvedDraftContent = _futureNhsContentService.ResolveDraftContent(draftContent);
                var resolvedPublishedContent = _futureNhsContentService.ResolvePublishedContent(publishedContent);

                // Get context content blocks
                var draftBlocks = publishedContent.Children.Select(x => _futureNhsContentService.ResolveDraftContent(_futureNhsContentService.GetDraftContent(x.Key, cancellationToken)));
                var publishedBlocks = publishedContent.Children.Select(x => _futureNhsContentService.ResolvePublishedContent(_futureNhsContentService.GetPublishedContent(x.Key, cancellationToken)));

                // For each block rollback to published version
                foreach (var block in draftBlocks)
                {
                    var draft = _futureNhsContentService.GetDraftContent(block.Item.Id, cancellationToken);
                    _futureNhsContentService.RollbackDraftContent(draft, cancellationToken);
                }

                // Find the difference between draft and published contents list of blocks.
                var blocksToRemove = _futureNhsContentService.CompareContentModelLists(draftBlocks, publishedBlocks);

                // Delete blocks.
                _futureNhsContentService.DeleteContent(blocksToRemove, cancellationToken);
            }

            // Rollback context content to published version.
            _futureNhsContentService.RollbackDraftContent(draftContent, cancellationToken);

            // Publish to reset draft status to false. 
            _futureNhsContentService.PublishContent(draftContent, cancellationToken);

            return new ApiResponse<string>().Success(contentId.ToString(), "Content discarded successfully.");
        }
    }
}
