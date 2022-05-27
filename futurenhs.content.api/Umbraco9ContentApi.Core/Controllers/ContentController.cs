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
        /// Gets the published content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/published")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModelData))]
        public IActionResult GetPublishedContent(Guid contentId, CancellationToken cancellationToken)
        {
            var result = _futureNhsContentHandler.GetPublishedContent(contentId, cancellationToken);

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
        /// Gets the content draft.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/draft")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModelData))]
        public ActionResult GetDraftContent(Guid contentId, CancellationToken cancellationToken)
        {
            var result = _futureNhsContentHandler.GetDraftContent(contentId, cancellationToken);

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
        /// Publishes the content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost("{contentId:guid}/publish")]
        public ActionResult PublishContent(Guid contentId, CancellationToken cancellationToken)
        {
            var result = _futureNhsContentHandler.PublishContentAndAssociatedContent(contentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error publishing content: " + contentId);
        }

        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("{contentId:guid}")]
        public ActionResult DeleteContent(Guid contentId, CancellationToken cancellationToken)
        {
            var result = _futureNhsContentHandler.DeleteContent(contentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error deleting content: " + contentId);
        }

        /// <summary>
        /// Discards the draft content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("{contentId:guid}/discard")]
        public ActionResult DiscardDraftContent(Guid contentId, CancellationToken cancellationToken)
        {
            var result = _futureNhsContentHandler.DiscardDraftContent(contentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error discarding draft content: " + contentId);
        }
    }
}

