namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Requests;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsBlockHandler
    {
        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<ContentModel> GetBlock(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets all blocks.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<IEnumerable<ContentModel>> GetAllBlocks(CancellationToken cancellationToken);
        /// <summary>
        /// Gets the block placeholder values.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="propertyGroupAlias">The property group alias.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<IEnumerable<string?>> GetBlockPlaceholderValues(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the block content.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<IEnumerable<string>> GetBlockContent(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the block lables.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<IEnumerable<string>> GetBlockLabels(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Creates the block.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<string> CreateBlock(CreateBlockRequest createRequest, CancellationToken cancellationToken);
    }
}
