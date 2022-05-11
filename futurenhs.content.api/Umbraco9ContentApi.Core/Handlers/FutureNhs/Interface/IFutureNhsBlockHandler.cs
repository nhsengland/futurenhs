namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Requests;
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
        /// <param name="propertyGroupAlias">The property group alias.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string?>>> GetBlockPlaceholderValuesAsync(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the block content asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockContentAsync(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the block lables asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockLabelsAsync(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Creates the block asynchronous.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreateBlockAsync(CreateBlockRequest createRequest, CancellationToken cancellationToken);
    }
}
