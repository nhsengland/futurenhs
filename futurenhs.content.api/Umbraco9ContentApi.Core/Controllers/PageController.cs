namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Requests;

    /// <summary>
    /// Page Api controller.
    /// </summary>
    [Route("api/page")]
    public sealed class PageController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;
        private readonly IFutureNhsPageHandler _futureNhsPageHandler;

        public PageController(IFutureNhsContentHandler futureNhsContentHandler, IFutureNhsPageHandler futureNhsPageHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
            _futureNhsPageHandler = futureNhsPageHandler;
        }

        /// <summary>
        /// Gets all pages asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public async Task<ActionResult> GetAllPagesAsync(CancellationToken cancellationToken)
        {
            var result = await _futureNhsContentHandler.GetAllPagesAsync(cancellationToken);

            if (result.Succeeded && !result.Data.Any())
            {
                return NotFound("No content found.");
            }

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error retrieving content.");
        }

        /// <summary>
        /// Updates the page asynchronous.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="pageModel">The page model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPut("{pageId:guid}")]
        public async Task<IActionResult> UpdatePageAsync(Guid pageId, PageModel pageModel, CancellationToken cancellationToken)
        {
            var result = await _futureNhsPageHandler.UpdatePageAsync(
                    pageId,
                    pageModel,
                    cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error updating the content.");
        }

        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult> CreatePageAsync([FromBody] CreatePageRequest createRequest, CancellationToken cancellationToken)
        {
            var result = await _futureNhsPageHandler.CreatePageAsync(createRequest.Name,
                createRequest.ParentId,
                cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error creating the content.");
        }

        /// <summary>
        /// Updates the user editing content asynchronous.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPut("{userId:guid}/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsPageHandler.UpdateUserEditingContentAsync(userId, pageId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error updating the content.");
        }

        /// <summary>
        /// Checks the page edit status asynchronous.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{pageId:guid}/editStatus")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public async Task<ActionResult> CheckPageEditStatusAsync(Guid pageId, CancellationToken cancellationToken)
        {
            var result = await _futureNhsPageHandler.CheckPageEditStatusAsync(pageId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error retrieving page edit status.");
        }
    }
}

