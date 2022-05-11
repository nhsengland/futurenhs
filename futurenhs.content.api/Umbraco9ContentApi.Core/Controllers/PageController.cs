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
        /// Gets all pages.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public  ActionResult GetAllPages(CancellationToken cancellationToken)
        {
            var result = _futureNhsPageHandler.GetAllPages(cancellationToken);

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
        /// Updates the page.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="pageModel">The page model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPut("{pageId:guid}")]
        public  IActionResult UpdatePage(Guid pageId, PageModel pageModel, CancellationToken cancellationToken)
        {
            var result = _futureNhsPageHandler.UpdatePage(
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
        /// Creates the content.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public  ActionResult CreatePage([FromBody] CreatePageRequest createRequest, CancellationToken cancellationToken)
        {
            var result = _futureNhsPageHandler.CreatePage(createRequest.Name,
                createRequest.ParentId,
                cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error creating the content.");
        }

        /// <summary>
        /// Updates the user editing content.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPut("{userId:guid}/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public  ActionResult UpdateUserEditingContent(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            var result = _futureNhsPageHandler.UpdateUserEditingContent(userId, pageId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error updating the content.");
        }

        /// <summary>
        /// Checks the page edit status.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{pageId:guid}/editStatus")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public  ActionResult CheckPageEditStatus(Guid pageId, CancellationToken cancellationToken)
        {
            var result = _futureNhsPageHandler.CheckPageEditStatus(pageId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Error retrieving page edit status.");
        }
    }
}

