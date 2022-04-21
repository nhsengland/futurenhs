namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Request;
    using System;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models;

    /// <summary>
    /// Content Api controller.
    /// </summary>
    [Route("api/content")]
    public sealed class ContentApiController : UmbracoApiController
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
        public async Task<ActionResult> GetContentAsync(Guid contentId)
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
        public async Task<ActionResult> GetAllContentAsync()
        {
            var response = await _futureNhsContentHandler.GetAllContentAsync();

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
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <returns>The created content.</returns>
        /// <remarks></remarks>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult> CreateContentAsync([FromBody] GeneralWebPageCreateRequest createRequest)
        {
            if (string.IsNullOrWhiteSpace(createRequest.Name))
            {
                return BadRequest("Page name not provided, please provide a page name.");
            }

            var response = await _futureNhsContentHandler.CreateContentAsync(
                createRequest.Name,
                createRequest.ParentId);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem(response.Message);
        }

        /// <summary>
        /// Publishes the specified content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The content identifier.</returns>
        /// <remarks></remarks>
        [HttpPost("{contentId:guid}/publish")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult> PublishContentAsync(Guid contentId)
        {
            var response = await _futureNhsContentHandler.PublishContentAsync(contentId);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem("Publish unsuccessful: " + contentId);
        }

        /// <summary>
        /// Updates the specified content.
        /// </summary>                                                                                                                                                                                                                                                           
        /// <param name="contentId">The content identifier.</param>
        /// <param name="updateRequest">The update request.</param>
        /// <returns>The content identifier.</returns>
        /// <remarks></remarks>
        [HttpPut("{contentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> UpdateContentAsync(Guid contentId, GeneralWebPageUpdateRequest updateRequest)
        {
            if (string.IsNullOrWhiteSpace(updateRequest.Title) && string.IsNullOrWhiteSpace(updateRequest.Description) && string.IsNullOrWhiteSpace(updateRequest.PageContent))
            {
                return BadRequest("No update provided, please check you are sending an update.");
            }

            var response = await _futureNhsContentHandler.UpdateContentAsync(
                    contentId,
                    updateRequest.Title,
                    updateRequest.Description,
                    updateRequest.PageContent);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem("Update unsuccessful: " + contentId);
        }

        /// <summary>
        /// Deletes the specified content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The content identifier.</returns>
        [HttpDelete("{contentId:guid}")]
        public async Task<ActionResult> DeleteContentAsync(Guid contentId)
        {
            var response = await _futureNhsContentHandler.DeleteContentAsync(contentId);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem("Deletion unsuccessful: " + contentId);
        }
    }
}
