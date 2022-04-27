namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Blocks;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsBlockHandler
    {
        /// <summary>
        /// Gets the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<BlockModel>> GetBlockAsync(Guid blockId);
        /// <summary>
        /// Gets all blocks.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<BlockModel>>> GetAllBlocksAsync();
        /// <summary>
        /// Gets the block placeholder values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockPlaceholderValuesAsync(Guid blockId);
    }
}
