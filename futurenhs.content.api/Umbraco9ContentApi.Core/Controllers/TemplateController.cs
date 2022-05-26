namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Content;

    /// <summary>
    /// Template Api controller.
    /// </summary>
    [Route("api/template")]
    public sealed class TemplateController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;
        private readonly IFutureNhsTemplateHandler _futureNhsTemplateHandler;

        public TemplateController(IFutureNhsContentHandler futureNhsContentHandler, IFutureNhsTemplateHandler futureNhsTemplateHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
            _futureNhsTemplateHandler = futureNhsTemplateHandler;
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{templateId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModelData))]
        public ActionResult GetTemplate(Guid templateId, CancellationToken cancellationToken)
        {
            var result = _futureNhsTemplateHandler.GetTemplate(templateId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModelData>))]
        public ActionResult GetAllTemplates(CancellationToken cancellationToken)
        {
            var result = _futureNhsTemplateHandler.GetAllTemplates(cancellationToken);

            if (result.Succeeded && !result.Data.Any())
            {
                return NotFound("No templates found.");
            }

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem(result.Message);
        }

        /// <summary>
        /// Deletes the template.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("{templateId:guid}")]
        public ActionResult DeleteTemplate(Guid templateId, CancellationToken cancellationToken)
        {
            var result = _futureNhsContentHandler.DeleteContent(templateId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return Problem("Deletion unsuccessful: " + templateId);
        }
    }
}