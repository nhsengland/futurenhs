namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    /// <summary>
    /// Block Api controller.
    /// </summary>
    [Route("api/block")]
    public sealed class BlockController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;
        private readonly IFutureNhsBlockHandler _futureNhsBlockHandler;

        public BlockController(IFutureNhsContentHandler futureNhsContentHandler, IFutureNhsBlockHandler futureNhsBlockHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
            _futureNhsBlockHandler = futureNhsBlockHandler;
        }

        /// <summary>
        /// Gets the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModel>))]
        public async Task<ActionResult> GetBlockAsync(Guid blockId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.GetContentPublishedAsync(blockId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all blocks asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ContentModel>>))]
        public async Task<ActionResult> GetAllBlocksAsync(CancellationToken cancellationToken)
        {
            var result = await _futureNhsBlockHandler.GetAllBlocksAsync(cancellationToken);

            if (result.Succeeded && !result.Data.Any())
            {
                return NotFound("No blocks found.");
            }

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem(result.Message);
        }

        /// <summary>
        /// Gets the block placeholder values for {propertyTypeAlias} asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="propertyGroupAlias">The property type alias.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/{propertyGroupAlias}/placeholder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<ActionResult> GetBlocPlaceholderValuesAsync(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(propertyGroupAlias))
                return BadRequest("No property group alias was provided.");

            var result = await _futureNhsBlockHandler.GetBlockPlaceholderValuesAsync(blockId, propertyGroupAlias, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the block content asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/content")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<ActionResult> GetBlockContentAsync(Guid blockId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsBlockHandler.GetBlockContentAsync(blockId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the block labels asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/labels")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<ActionResult> GetBlockLabelsAsync(Guid blockId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsBlockHandler.GetBlockLabelsAsync(blockId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        /// <summary>
        /// Deletes the specified block.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("{blockId:guid}")]
        public async Task<ActionResult> DeleteBlockAsync(Guid blockId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.DeleteContentAsync(blockId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem(result.Message);
        }
    }
}
