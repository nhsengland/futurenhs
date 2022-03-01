namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Request;
    using System;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using UmbracoContentApi.Core.Models;

    /// <summary>
    /// Content Api controller.
    /// </summary>
    [Route("api/content")]
    public class ContentApiController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentApiController" /> class.
        /// </summary>
        /// <param name="futureNhsContentHandler">The future Nhs content handler.</param>
        public ContentApiController(IFutureNhsContentHandler futureNhsContentHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
        }

        /// <summary>
        /// Gets the specified content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The specified content.</returns>
        /// <remarks></remarks>
        [HttpGet("{contentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> GetAsync(Guid contentId)
        {
            var content = await _futureNhsContentHandler.GetContentAsync(contentId);

            if (content is null)
            {
                return NotFound();
            }

            return Ok(content);
        }

        /// <summary>
        /// Gets all content.
        /// </summary>
        /// <returns>All content.</returns>
        /// <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public async Task<ActionResult> GetAllAsync()
        {
            var content = await _futureNhsContentHandler.GetAllContentAsync();

            if (content is null || !content.Any())
            {
                return NotFound();
            }

            return Ok(content);
        }

        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageParentId">The page parent identifier.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns>Created page identifier.</returns>
        /// <remarks></remarks>
        [HttpPost("{pageName}/create")]
        public async Task<ActionResult> CreateAsync(string pageName, string? pageParentId = null, [FromBody()] bool publish = false)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return BadRequest("Page name not provided, please provide a page name.");
            }

            var result = await _futureNhsContentHandler.CreateContentAsync(pageName, pageParentId, publish);

            if (result is not null)
            {
                return Ok(result.Key);
            }

            return Problem("Error creating the page, content was null.");
        }

        /// <summary>
        /// Publishes the specified content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The content identifier.</returns>
        /// <remarks></remarks>
        [HttpPost("{contentId:guid}/publish")]
        public async Task<ActionResult> PublishAsync(Guid contentId)
        {
            var result = await _futureNhsContentHandler.PublishContentAsync(contentId);

            if (result)
            {
                return Ok("Successfully published: " + contentId);
            }

            return Problem("Publish unsuccessful: " + contentId);
        }

        /// <summary>
        /// Updates the specified request.
        /// </summary>                                                                                                                                                                                                                                                           
        /// <param name="contentId">The content identifier.</param>
        /// <param name="request">The update request.</param>
        /// <returns>The content identifier.</returns>
        /// <remarks></remarks>
        /// <response code="200">Succees.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not found.</response>
        /// <response code="500">Error.</response>
        [HttpPost("{contentId:guid}/update")]
        public async Task<IActionResult> UpdateAsync(Guid contentId, GeneralWebPageUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) && string.IsNullOrWhiteSpace(request.Description) && string.IsNullOrWhiteSpace(request.PageContent))
            {
                return BadRequest("No update provided, please check you are sending an update.");
            }

            var result = await _futureNhsContentHandler.UpdateContentAsync(
                    contentId,
                    request.Title,
                    request.Description,
                    request.PageContent);

            if (result)
            {
                return Ok("Update successful: " + contentId);
            }

            return Problem("Update unsuccessful: " + contentId);
        }

        /// <summary>
        /// Deletes the specified content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The content identifier.</returns>
        [HttpDelete("{contentId:guid}")]
        public async Task<ActionResult> DeleteAsync(Guid contentId)
        {
            var result = await _futureNhsContentHandler.DeleteContentAsync(contentId);

            if (result)
            {
                return Ok("Successfully deleted: " + contentId);
            }

            return Problem("Deletion unsuccessful: " + contentId);
        }
    }
}
