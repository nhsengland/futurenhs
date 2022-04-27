namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsBlockHandler
    {
        /// <summary>
        /// Gets the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetBlockAsync(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets all blocks asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllBlocksAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Gets the block placeholder values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockPlaceholderValuesAsync(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the block field values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockFieldValuesAsync(Guid blockId, CancellationToken cancellationToken);

    }
}
