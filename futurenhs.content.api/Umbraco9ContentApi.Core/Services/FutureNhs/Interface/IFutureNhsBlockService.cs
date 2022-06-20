using Umbraco.Cms.Core.Models;
using Umbraco9ContentApi.Core.Models.Requests;
using ContentModelData = Umbraco9ContentApi.Core.Models.Content.ContentModelData;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsBlockService
    {
        /// <summary>
        /// Gets the block placeholder values.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="propertyGroupAlias">The property group alias.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        IEnumerable<string> GetBlockPlaceholderValues(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken);
        /// <summary>
        /// Creates the block.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        IContent CreateBlock(CreateBlockRequest createRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        IContent UpdateBlock(ContentModelData block, int sortOrder, CancellationToken cancellationToken);
        /// <summary>
        /// Gets all the descendent block ids.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        IEnumerable<ContentModelData> GetAllDescendentBlockIds(IEnumerable<ContentModelData> blocks, CancellationToken cancellationToken);
    }
}
