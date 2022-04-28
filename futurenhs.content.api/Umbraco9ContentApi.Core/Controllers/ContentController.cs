namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Request;
    using System;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Requests;

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
        /// Gets the content published asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/published")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> GetContentPublishedAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var content = await _futureNhsContentHandler.GetContentPublishedAsync(contentId, cancellationToken);

            if (content is null)
            {
                return NotFound();
            }

            return Ok(content);
        }

        /// <summary>
        /// Gets the content draft asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/draft")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> GetContentDraftAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var content = await _futureNhsContentHandler.GetContentDraftAsync(contentId, cancellationToken);

            if (content is null)
            {
                return NotFound();
            }

            return Ok(content);
        }

        /// <summary>
        /// Gets all content asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public async Task<ActionResult> GetAllContentAsync(CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.GetAllContentAsync(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error retrieving content.");
        }

        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult> CreateContentAsync([FromBody] GeneralWebPageCreateRequest createRequest, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.CreateContentAsync(createRequest.PageName,
                createRequest.PageParentId,
                cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error creating the content.");
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
        /// Updates the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPut("{contentId:guid}/update")]
        public async Task<IActionResult> UpdateContentAsync(Guid contentId, GeneralWebPageUpdateRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title) && string.IsNullOrWhiteSpace(request.Description) && request.PageContent is null)
            {
                return BadRequest("No update provided, please check you are sending an update.");
            }

            var result = await _futureNhsContentHandler.UpdateContentAsync(
                    contentId,
                    request.Title,
                    request.Description,
                    request.PageContent,
                    cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error updating content: " + contentId);
        }

        [HttpPut("{contentId:guid}")]
        public async Task<IActionResult> UpdateContentAsync(Guid contentId, PageContentModel pageContentModel, CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.UpdateContentAsync(
                    contentId,
                    pageContentModel,
                    cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error updating content: " + contentId);
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
    }
}

