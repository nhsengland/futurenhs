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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModel>))]
        public async Task<ActionResult> GetBlockAsync(Guid blockId)
        {
            var content = await _futureNhsBlockHandler.GetBlockAsync(blockId);

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
        /// Gets the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/placeholder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<ActionResult> GetBlockPlaceholderValuesAsync(Guid blockId)
        {
            var content = await _futureNhsBlockHandler.GetBlockPlaceholderValuesAsync(blockId);

            if (content is null)
            {
                return NotFound();
            }

            return Ok(content);
        }

        /// <summary>
        /// Gets the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/fields")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<ActionResult> GetBlockFieldValuesAsync(Guid blockId)
        {
            var content = await _futureNhsBlockHandler.GetBlockFieldValuesAsync(blockId);

            if (content is null)
            {
                return NotFound();
            }

            return Ok(content);
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
