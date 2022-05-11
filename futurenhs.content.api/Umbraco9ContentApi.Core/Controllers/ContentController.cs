namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Content;

    /// <summary>
    /// Content Api controller.
    /// </summary>
    [Route("api/content")]
    public sealed class ContentController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;

        public ContentController(IFutureNhsContentHandler futureNhsContentHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
        }

        /// <summary>
        /// Gets the published content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/published")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<IActionResult> GetPublishedContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.GetPublishedContentAsync(contentId, cancellationToken);

            if (result.Succeeded == false)
            {
                return NotFound(result);
            }

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error retrieving content.");
        }

        /// <summary>
        /// Gets the content draft asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/draft")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> GetDraftContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.GetDraftContentAsync(contentId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error retrieving content.");
        }

        /// <summary>
        /// Publishes the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost("{contentId:guid}/publish")]
        public async Task<ActionResult> PublishContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.PublishContentAsync(contentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error publishing content: " + contentId);
        }

        /// <summary>
        /// Deletes the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("{contentId:guid}")]
        public async Task<ActionResult> DeleteContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.DeleteContentAsync(contentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error deleting content: " + contentId);
        }

        /// <summary>
        /// Discards the draft content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("{contentId:guid}/discard")]
        public async Task<ActionResult> DiscardDraftContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.DiscardDraftContentAsync(contentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error discarding draft content: " + contentId);
        }
    }
}

