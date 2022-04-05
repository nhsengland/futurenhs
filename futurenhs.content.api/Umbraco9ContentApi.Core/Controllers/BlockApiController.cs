namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Response;
    using UmbracoContentApi.Core.Models;

    /// <summary>
    /// Block Api controller.
    /// </summary>
    [Route("api/block")]
    public sealed class BlockApiController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;
        private readonly IFutureNhsBlockHandler _futureNhsBlockHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockApiController"/> class.
        /// </summary>
        /// <param name="futureNhsContentHandler">The future Nhs content handler.</param>
        /// <param name="futureNhsBlockHandler">The future Nhs block handler.</param>
        public BlockApiController(IFutureNhsContentHandler futureNhsContentHandler, IFutureNhsBlockHandler futureNhsBlockHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
            _futureNhsBlockHandler = futureNhsBlockHandler;
        }

        /// <summary>
        /// Gets the specified block.
        /// </summary>
        /// <param name="blockId">The content identifier.</param>
        /// <returns>The specified block.</returns>
        /// <remarks></remarks>
        [HttpGet("{blockId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> GetBlockAsync(Guid blockId)
        {
            var content = await _futureNhsContentHandler.GetContentAsync(blockId);

            if (content is null)
            {
                return NotFound();
            }

            return Ok(content);
        }

        /// <summary>
        /// Gets all blocks.
        /// </summary>
        /// <returns>All blocks.</returns>
        /// <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ContentModel>>))]
        public async Task<ActionResult> GetAllBlocksAsync()
        {
            var response = await _futureNhsBlockHandler.GetAllBlocksAsync();

            if (response.Succeeded && !response.Payload.Any())
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
        /// Deletes the specified block.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns>The block identifier.</returns>
        /// <remarks></remarks>
        [HttpDelete("{blockId:guid}")]
        public async Task<ActionResult> DeleteBlockAsync(Guid blockId)
        {
            var response = await _futureNhsContentHandler.DeleteContentAsync(blockId);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem(response.Message);
        }
    }
}
