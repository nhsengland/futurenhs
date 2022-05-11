namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsValidationService _futureNhsValidationService;
        private List<string> errorList = new List<string>();

        public FutureNhsContentHandler(IConfiguration config, IFutureNhsContentService futureNhsContentService, IFutureNhsValidationService futureNhsValidationService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _futureNhsContentService = futureNhsContentService ?? throw new ArgumentNullException(nameof(futureNhsContentService));
            _futureNhsValidationService = futureNhsValidationService ?? throw new ArgumentNullException(nameof(futureNhsValidationService));
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> PublishContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            bool result;

            var publishedContent = await _futureNhsContentService.GetPublishedContentAsync(contentId, cancellationToken);
            var draftContent = await _futureNhsContentService.GetDraftContentAsync(contentId, cancellationToken);

            if (draftContent is not null && publishedContent is not null)
            {
                var resolvedDraftContent = await _futureNhsContentService.ResolveDraftContentAsync(draftContent, cancellationToken);
                var resolvedPublishedContent = await _futureNhsContentService.ResolvePublishedContentAsync(publishedContent, "content", cancellationToken);

                var draftBlocks = resolvedDraftContent.Content.Where(x => x.Key == "blocks").Select(c => c.Value).FirstOrDefault() as IEnumerable<ContentModel>;
                var publishedBlocks = resolvedPublishedContent.Content.Where(x => x.Key == "blocks").Select(c => c.Value).FirstOrDefault() as IEnumerable<ContentModel>;

                if (publishedBlocks is not null && draftBlocks is not null)
                {
                    // Find the difference between published and draft content.
                    var difference = publishedBlocks.Select(x => x.Item.Id).Where(x => !draftBlocks.Select(pb => pb.Item.Id).Contains(x)).ToList();

                    // Delete the difference if they were initially draft.
                    foreach (var blockId in difference)
                    {
                        result = await _futureNhsContentService.DeleteAssociatedContent(blockId, cancellationToken);

                        if (!result)
                        {
                            errorList.Add("Could not delete associated content.");
                            response.Failure(errorList, "Failed");
                        }

                        result = await _futureNhsContentService.DeleteContentAsync(blockId, cancellationToken);
                    }

                    // Publish remaining draft blocks.
                    foreach (var draftBlock in draftBlocks)
                    {
                        result = await _futureNhsContentService.PublishContentAsync(draftBlock.Item.Id, cancellationToken);

                        if (!result)
                        {
                            errorList.Add("Could not publish associated blocks.");
                            response.Failure(errorList, "Failed");
                        }
                    }
                }
            }

            result = await _futureNhsContentService.PublishContentAsync(contentId, cancellationToken);

            if (result)
            {
                return response.Success(result.ToString(), "Success.");
            }

            errorList.Add("Publish failed.");
            return response.Failure(errorList, "Failed.");

        }

        /// <inheritdoc />
        public async Task<ApiResponse<ContentModel>> GetPublishedContentAsync(Guid id, CancellationToken cancellationToken)
        {
            ApiResponse<ContentModel> response = new ApiResponse<ContentModel>();
            var content = await _futureNhsContentService.GetPublishedContentAsync(id, cancellationToken);
            var result = await _futureNhsContentService.ResolvePublishedContentAsync(content, "content", cancellationToken);

            if (result is not null)
            {
                return response.Success(result, "Success.");
            }

            errorList.Add("Could not retrieve content.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<ContentModel>> GetDraftContentAsync(Guid id, CancellationToken cancellationToken)
        {
            ApiResponse<ContentModel> response = new ApiResponse<ContentModel>();
            var content = await _futureNhsContentService.GetDraftContentAsync(id, cancellationToken);
            var result = await _futureNhsContentService.ResolveDraftContentAsync(content, cancellationToken);

            if (result is not null)
            {
                return response.Success(result, "Success.");
            }

            errorList.Add("Could not retrieve content.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> DeleteContentAsync(Guid id, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var result = await _futureNhsContentService.DeleteContentAsync(id, cancellationToken);

            if (result)
            {
                return response.Success(id.ToString(), "Success.");
            }

            errorList.Add("Could not delete content.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<ContentModel>>> GetAllPagesAsync(CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<ContentModel>> response = new ApiResponse<IEnumerable<ContentModel>>();
            var contentModels = new List<ContentModel>();
            var pagesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");
            var publishedContent = await _futureNhsContentService.GetPublishedContentChildrenAsync(pagesFolderGuid, cancellationToken);

            if (publishedContent is not null && publishedContent.Any())
            {
                foreach (var content in publishedContent)
                {
                    contentModels.Add(await _futureNhsContentService.ResolvePublishedContentAsync(content, "content", cancellationToken));
                }
            }

            return response.Success(contentModels, "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> DiscardDraftContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            bool result;

            var draftContent = await _futureNhsContentService.GetDraftContentAsync(contentId, cancellationToken);

            if (draftContent.ContentType.Alias == GeneralWebPage.ModelTypeAlias)
            {
                var publishedContent = await _futureNhsContentService.GetPublishedContentAsync(contentId, cancellationToken);

                var resolvedDraftContent = await _futureNhsContentService.ResolveDraftContentAsync(draftContent, cancellationToken);
                var resolvedPublishedContent = await _futureNhsContentService.ResolvePublishedContentAsync(publishedContent, "content", cancellationToken);

                // get content list
                var draftBlocks = resolvedDraftContent.Content.Where(x => x.Key == "blocks").Select(c => c.Value).FirstOrDefault() as IEnumerable<ContentModel>;
                var publishedBlocks = resolvedPublishedContent.Content.Where(x => x.Key == "blocks").Select(c => c.Value).FirstOrDefault() as IEnumerable<ContentModel>;

                if (publishedBlocks is not null && draftBlocks is not null)
                {
                    // Rollback associated draft content to published version.
                    foreach (var block in draftBlocks)
                    {
                        var draft = await _futureNhsContentService.GetDraftContentAsync(block.Item.Id, cancellationToken);
                        result = _futureNhsContentService.RollbackDraftContentAsync(draft, cancellationToken);

                        if (!result)
                        {
                            errorList.Add("Could not rollback associated content.");
                            response.Failure(errorList, "Failed");
                        }
                    }

                    // Find the difference between published and draft content.
                    var difference = draftBlocks.Select(x => x.Item.Id).Where(x => !publishedBlocks.Select(pb => pb.Item.Id).Contains(x)).ToList();

                    // Delete the difference if they were originally published.
                    foreach (var blockId in difference)
                    {
                        result = await _futureNhsContentService.DeleteAssociatedContent(blockId, cancellationToken);

                        if (!result)
                        {
                            errorList.Add("Could not delete associated content.");
                            response.Failure(errorList, "Failed");
                        }

                        result = await _futureNhsContentService.DeleteContentAsync(blockId, cancellationToken);
                    }
                }
            }

            // Rollback draft content to published version.
            result = _futureNhsContentService.RollbackDraftContentAsync(draftContent, cancellationToken);

            // Publish to reset draft status to false. 
            result = await _futureNhsContentService.PublishContentAsync(draftContent.Key, cancellationToken);

            if (result)
            {
                return response.Success(result.ToString(), "Success.");
            }

            errorList.Add("Could not discard all or some content.");
            return response.Failure(errorList, "Failed.");
        }
    }
}
