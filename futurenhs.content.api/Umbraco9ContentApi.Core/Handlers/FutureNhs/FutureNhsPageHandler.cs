namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Services.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Response;
    using ContentModelData = Models.Content.ContentModelData;

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
            Guid pageParentGuid;
            var pageFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");

            // If a parent page id is supplied and is a valid guid, set that page as the page parent. Else use the pages folder.
            Guid parentId = pageParentId is not null && Guid.TryParse(pageParentId, out pageParentGuid)
                ? pageParentGuid
                : pageFolderGuid;

            var content = _futureNhsContentService.CreateContentFromTemplate(pageName, parentId, Guid.Parse("0B955A4A-9E26-43E8-BB4B-51010E264D64"), cancellationToken); // Guid = homepage template (Umbraco content)

            return new ApiResponse<string>().Success(content.Key.ToString(), "Page created successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<string> UpdatePage(Guid pageId, PageModel pageModel, CancellationToken cancellationToken)
        {
            // Get the draft page to update
            var pageToUpdate = _futureNhsContentService.GetDraftContent(pageId, cancellationToken);
            var pagePublished = _futureNhsContentService.GetPublishedContent(pageId, cancellationToken);

            // Get current draft blocks
            var resolvedDraftBlocks = pagePublished.Children.Select(x => _futureNhsContentService.ResolveDraftContent(_futureNhsContentService.GetDraftContent(x.Key, cancellationToken)));

            // Get page model block child blocks
            var pageModelBlocksContentModelDataList = _futureNhsBlockService.GetAllDescendentBlockIds(pageModel.Blocks, cancellationToken).ToList();

            // Remove any blocks that were on the latest saved draft but not the new incoming draft
            if (resolvedDraftBlocks is not null && resolvedDraftBlocks is IEnumerable<ContentModelData> draftBlocks && draftBlocks.Any())
            {
                // Get latest saved draft block child blocks
                var savedDraftBlocksChildBlocks = _futureNhsBlockService.GetAllDescendentBlockIds(draftBlocks, cancellationToken);

                // Find the difference between saved draft and incoming page model blocks
                var blocksToRemove = _futureNhsContentService.CompareContentModelLists(savedDraftBlocksChildBlocks, pageModelBlocksContentModelDataList);

                foreach (var blockId in blocksToRemove)
                {
                    _futureNhsContentService.DeleteContent(blockId, cancellationToken);
                }
            }

            // Update all page descendant blocks
            if (pageModelBlocksContentModelDataList is not null && pageModelBlocksContentModelDataList.Any())
            {
                for (int i = 0; i < pageModelBlocksContentModelDataList.Count(); i++)
                {
                    var content = pageModelBlocksContentModelDataList[i];
                    var sortOtder = i;

                    var update = _futureNhsBlockService.UpdateBlock(content, sortOtder, cancellationToken);
                    _futureNhsContentService.SaveContent(update, cancellationToken);
                }
            }

            // Save the page
            _futureNhsContentService.SaveContent(pageToUpdate, cancellationToken);

            return new ApiResponse<string>().Success(pageId.ToString(), "Page updated successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<IEnumerable<ContentModelData>> GetAllPages(CancellationToken cancellationToken)
        {
            var contentModels = new List<ContentModelData>();
            var pagesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");
            var publishedContent = _futureNhsContentService.GetPublishedContent(pagesFolderGuid, cancellationToken).Children;

            if (publishedContent is not null && publishedContent.Any())
            {
                foreach (var content in publishedContent)
                {
                    contentModels.Add(_futureNhsContentService.ResolvePublishedContent(content));
                }
            }

            return new ApiResponse<IEnumerable<ContentModelData>>().Success(contentModels, "All pages retrieved successfully.");
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
