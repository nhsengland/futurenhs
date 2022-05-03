namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Services.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    /// <summary>
    /// The handler that handles block methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsBlockHandler" />
    public sealed class FutureNhsBlockHandler : IFutureNhsBlockHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsBlockService _futureNhsBlockService;
        private List<string> errorList = new List<string>();

        public FutureNhsBlockHandler(IFutureNhsContentService futureNhsContentService, IFutureNhsBlockService futureNhsBlockService, IConfiguration config)
        {
            _futureNhsContentService = futureNhsContentService;
            _futureNhsBlockService = futureNhsBlockService;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<ContentModel>>> GetAllBlocksAsync(CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<ContentModel>> response = new ApiResponse<IEnumerable<ContentModel>>();
            var ContentModels = new List<ContentModel>();
            var blocksFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Blocks");
            var publishedBlocks = await _futureNhsContentService.GetPublishedContentChildrenAsync(blocksFolderGuid, cancellationToken);

            if (publishedBlocks is not null && publishedBlocks.Any())
            {
                foreach (var block in publishedBlocks)
                {
                    ContentModels.Add(await _futureNhsContentService.ResolvePublishedContentAsync(block, "content", cancellationToken));
                }
            }

            return response.Success(ContentModels, "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<ContentModel>> GetBlockAsync(Guid blockId, CancellationToken cancellationToken)
        {
            ApiResponse<ContentModel> response = new ApiResponse<ContentModel>();
            var block = await _futureNhsContentService.GetPublishedContentAsync(blockId, cancellationToken);
            var result = await _futureNhsContentService.ResolvePublishedContentAsync(block, "content", cancellationToken);

            if (result is not null)
            {
                return response.Success(result, "Success.");
            }

            errorList.Add("Could not retrieve block.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<string?>>> GetBlockPlaceholderValuesAsync(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<string?>> response = new ApiResponse<IEnumerable<string?>>();
            var blockPlaceholderValues = await _futureNhsBlockService.GetBlockPlaceholderValuesAsync(blockId, propertyGroupAlias, cancellationToken);
            return response.Success(blockPlaceholderValues, "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<string>>> GetBlockContentAsync(Guid blockId, CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<string>> response = new ApiResponse<IEnumerable<string>>();
            var block = await _futureNhsContentService.GetPublishedContentAsync(blockId, cancellationToken);
            var result = await _futureNhsContentService.ResolvePublishedContentAsync(block, "content", cancellationToken);

            if (result is not null)
            {
                return response.Success(result.Content.Keys, "Success.");
            }

            errorList.Add("Could not retrieve block content.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<string>>> GetBlockLabelsAsync(Guid blockId, CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<string>> response = new ApiResponse<IEnumerable<string>>();
            var block = await _futureNhsContentService.GetPublishedContentAsync(blockId, cancellationToken);
            var result = await _futureNhsContentService.ResolvePublishedContentAsync(block, "labels", cancellationToken);

            if (result is not null)
            {
                return response.Success(result.Content.Keys, "Success.");
            }

            errorList.Add("Could not retrieve block labels.");
            return response.Failure(errorList, "Failed.");
        }
    }
}
