namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Services.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Blocks;
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
        private List<string>? errorList = new List<string>();

        public FutureNhsBlockHandler(IFutureNhsContentService futureNhsContentService, IFutureNhsBlockService futureNhsBlockService, IConfiguration config)
        {
            _futureNhsContentService = futureNhsContentService;
            _futureNhsBlockService = futureNhsBlockService;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<BlockModel>>> GetAllBlocksAsync()
        {
            ApiResponse<IEnumerable<BlockModel>> response = new ApiResponse<IEnumerable<BlockModel>>();
            var blockModels = new List<BlockModel>();
            var blocksFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Blocks");
            var publishedBlocks = await _futureNhsContentService.GetPublishedChildrenAsync(blocksFolderGuid);

            if (publishedBlocks is not null && publishedBlocks.Any())
            {
                foreach (var block in publishedBlocks)
                {
                    blockModels.Add(await _futureNhsBlockService.ResolvePublishedBlockAsync(block));
                }
            }

            return response.Success(blockModels, "Success.");
        }

        public async Task<ApiResponse<BlockModel>> GetBlockAsync(Guid blockId)
        {
            ApiResponse<BlockModel> response = new ApiResponse<BlockModel>();
            var block = await _futureNhsContentService.GetPublishedAsync(blockId);
            var result = await _futureNhsBlockService.ResolvePublishedBlockAsync(block);

            if (result is not null)
            {
                return response.Success(result, "Success.");
            }

            errorList.Add("Couldn't retrieve block.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<string>>> GetBlockPlaceholderValuesAsync(Guid blockId)
        {
            ApiResponse<IEnumerable<string>> response = new ApiResponse<IEnumerable<string>>();
            var blockPlaceholderValues = await _futureNhsBlockService.GetBlockPlaceholderValuesAsync(blockId);
            return response.Success(blockPlaceholderValues, "Success.");
        }
    }
}
