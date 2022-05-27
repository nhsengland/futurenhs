namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Requests;
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
        /// Creates the block.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateBlock(CreateBlockRequest createRequest, CancellationToken cancellationToken)
        {
            var result = _futureNhsBlockHandler.CreateBlock(createRequest, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem(result.Message);
        }

        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModelData>))]
        public ActionResult GetBlock(Guid blockId, CancellationToken cancellationToken)
        {
            var result = _futureNhsContentHandler.GetPublishedContent(blockId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all blocks.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ContentModelData>>))]
        public ActionResult GetAllBlocks(CancellationToken cancellationToken)
        {
            var result = _futureNhsBlockHandler.GetAllBlocks(cancellationToken);

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
        /// Gets the block placeholder values for {propertyTypeAlias}.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="propertyGroupAlias">The property type alias.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/{propertyGroupAlias}/placeholder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public ActionResult GetBlocPlaceholderValues(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(propertyGroupAlias))
                return BadRequest("No property group alias was provided.");

            var result = _futureNhsBlockHandler.GetBlockPlaceholderValues(blockId, propertyGroupAlias, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the block content.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/content")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public ActionResult GetBlockContent(Guid blockId, CancellationToken cancellationToken)
        {
            var result = _futureNhsBlockHandler.GetBlockContent(blockId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the block labels.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{blockId:guid}/labels")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public ActionResult GetBlockLabels(Guid blockId, CancellationToken cancellationToken)
        {
            var result = _futureNhsBlockHandler.GetBlockLabels(blockId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);

        }
    }
}
