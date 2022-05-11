namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Services.FutureNhs.Interface;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using Umbraco9ContentApi.Core.Extensions;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Response;
    using ContentModel = Models.Content.ContentModel;

    /// <summary>
    /// The handler that handles page methods.
    /// </summary>
    /// <seealso cref="IFutureNhsContentHandler" />
    public sealed class FutureNhsPageHandler : IFutureNhsPageHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsBlockService _futureNhsBlockService;
        private readonly IFutureNhsValidationService _futureNhsValidationService;

        private List<string> errorList = new();

        public FutureNhsPageHandler(IConfiguration config, IFutureNhsContentService futureNhsContentService, IFutureNhsBlockService futureNhsBlockService, IFutureNhsValidationService futureNhsValidationService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _futureNhsContentService = futureNhsContentService ?? throw new ArgumentNullException(nameof(futureNhsContentService));
            _futureNhsBlockService = futureNhsBlockService ?? throw new ArgumentNullException(nameof(futureNhsBlockService));
            _futureNhsValidationService = futureNhsValidationService ?? throw new ArgumentNullException(nameof(futureNhsValidationService)); ;
        }

        /// <inheritdoc />
        public ApiResponse<string> CreatePage(string pageName, string pageParentId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            Guid pageParentGuid;
            var pageFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");

            // If a parent page id is supplied and is a valid guid, set that page as the page parent. Else use the pages folder.
            Guid parentId = pageParentId is not null && Guid.TryParse(pageParentId, out pageParentGuid)
                ? pageParentGuid
                : pageFolderGuid;

            return new ApiResponse<string>().Success(_futureNhsContentService.CreateContent(pageName, parentId, GeneralWebPage.ModelTypeAlias, cancellationToken).Key.ToString(), "Page created successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<string> UpdatePage(Guid pageId, PageModel pageModel, CancellationToken cancellationToken)
        {
            List<string> pageBlockUdis = new();

            // Get the draft page to update
            var pageToUpdate = _futureNhsContentService.GetDraftContent(pageId, cancellationToken);

            // Get current saved draft
            var resolvedDraftContent = _futureNhsContentService.ResolveDraftContent(pageToUpdate, cancellationToken);
            var resolvedDraftBlocks = resolvedDraftContent.Content.Where(x => x.Key == "blocks").Select(c => c.Value).FirstOrDefault();

            // Get page model block child blocks
            var pageModelBlockChildBlocks = _futureNhsBlockService.GetChildBlocks(pageModel.Blocks, cancellationToken);

            // Remove any blocks that were on the latest saved draft but not the new incoming draft
            if (resolvedDraftBlocks is not null && resolvedDraftBlocks is IEnumerable<ContentModel> draftBlocks && draftBlocks.Any())
            {
                // Get latest saved draft block child blocks
                var savedDraftBlocksChildBlocks = _futureNhsBlockService.GetChildBlocks(draftBlocks, cancellationToken);

                // Find the difference between saved draft and incoming page model block child blocks
                var blocksToRemove = _futureNhsContentService.CompareContentModelLists(savedDraftBlocksChildBlocks, pageModelBlockChildBlocks);

                foreach (var blockId in blocksToRemove)
                {
                    _futureNhsContentService.DeleteContent(blockId, cancellationToken);
                }
            }

            // If the incoming page update blocks have child blocks, update those too.
            if (pageModelBlockChildBlocks is not null && pageModelBlockChildBlocks.Any())
            {
                foreach (var content in pageModelBlockChildBlocks)
                {
                    var update = _futureNhsBlockService.UpdateBlock(content, cancellationToken);
                    _futureNhsContentService.SaveContent(update, cancellationToken);
                }
            }

            // Update the blocks and child blocks on the page
            foreach (var block in pageModel.Blocks)
            {
                _futureNhsValidationService.ValidateContentModel(block);

                var updatedBlock = _futureNhsBlockService.UpdateBlock(block, cancellationToken);
                _futureNhsContentService.SaveContent(updatedBlock, cancellationToken);

                // Add updated block to the list of blocks on this page 
                pageBlockUdis.Add(block.GetUdi());
            }

            // Set the list of blocks on the page
            var updateContentPropertyResult = _futureNhsContentService.SetContentPropertyValue(pageToUpdate, "blocks", string.Join(",", pageBlockUdis), cancellationToken);

            // Save the page
            _futureNhsContentService.SaveContent(updateContentPropertyResult, cancellationToken);

            return new ApiResponse<string>().Success(pageId.ToString(), "Page updated successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<IEnumerable<ContentModel>> GetAllPages(CancellationToken cancellationToken)
        {
            var contentModels = new List<ContentModel>();
            var pagesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");
            var publishedContent = _futureNhsContentService.GetPublishedContent(pagesFolderGuid, cancellationToken).Children;

            if (publishedContent is not null && publishedContent.Any())
            {
                foreach (var content in publishedContent)
                {
                    contentModels.Add(_futureNhsContentService.ResolvePublishedContent(content, "content", cancellationToken));
                }
            }

            return new ApiResponse<IEnumerable<ContentModel>>().Success(contentModels, "All pages retrieved successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<string> UpdateUserEditingContent(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            var page = _futureNhsContentService.GetDraftContent(pageId, cancellationToken);
            var updatedPage = _futureNhsContentService.SetContentPropertyValue(page, "userEditing", page, cancellationToken);
            _futureNhsContentService.SaveContent(updatedPage, cancellationToken);
            return new ApiResponse<string>().Success(pageId.ToString(), "Page updated successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<string> CheckPageEditStatus(Guid pageId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var draftPage = _futureNhsContentService.GetDraftContent(pageId, cancellationToken);

            if (!draftPage.Edited)
            {
                return response.Success(string.Empty, "Success.");
            }

            var userId = draftPage.Properties.Where(x => x.Alias == "userEditing")
                ?.Select(p => p.GetValue())
                ?.FirstOrDefault()
                ?.ToString();

            return string.IsNullOrEmpty(userId)
                ? response.Failure(null, "Failed to retrieve userId")
                : response.Success(userId, "Success.");
        }
    }
}
