namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Response;

    /// <summary>
    /// Block Api controller.
    /// </summary>
    [Route("api/block")]
    public sealed class BlockController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;
        private readonly IFutureNhsBlockHandler _futureNhsBlockHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockController"/> class.
        /// </summary>
        /// <param name="futureNhsContentHandler">The future Nhs content handler.</param>
        /// <param name="futureNhsBlockHandler">The future Nhs block handler.</param>
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> GetBlockAsync(Guid blockId, CancellationToken cancellationToken)
        {
            var content = await _futureNhsContentHandler.GetContentPublishedAsync(blockId, cancellationToken);

            if (content is null)
            {
                return NotFound();
            }

            return Ok(content);
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
            var response = await _futureNhsBlockHandler.GetAllBlocksAsync(cancellationToken);

            if (response.Succeeded && !response.Data.Any())
            {
                return NotFound("No blocks found.");
            }

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem(response.Message);
        }

        /// <summary>
        /// Deletes the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("{blockId:guid}")]
        public async Task<ActionResult> DeleteBlockAsync(Guid blockId, CancellationToken cancellationToken)
        {
            var response = await _futureNhsContentHandler.DeleteContentAsync(blockId, cancellationToken);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem(response.Message);
        }
    }
}
