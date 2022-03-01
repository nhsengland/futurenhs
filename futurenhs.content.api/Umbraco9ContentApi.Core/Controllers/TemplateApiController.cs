namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using UmbracoContentApi.Core.Models;

    /// <summary>
    /// Template Api controller.
    /// </summary>
    [Route("api/template")]
    public class TemplateApiController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;
        private readonly IFutureNhsTemplateHandler _futureNhsTemplateHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateApiController"/> class.
        /// </summary>
        /// <param name="futureNhsContentHandler">The future Nhs content handler.</param>
        /// <param name="futureNhsTemplateHandler">The future Nhs template handler.</param>
        public TemplateApiController(IFutureNhsContentHandler futureNhsContentHandler, IFutureNhsTemplateHandler futureNhsTemplateHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
            _futureNhsTemplateHandler = futureNhsTemplateHandler;
        }

        /// <summary>
        /// Gets the specified template identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns>The specified template.</returns>
        /// <remarks></remarks>
        [HttpGet("{templateId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> Get(Guid templateId)
        {
            var template = await _futureNhsTemplateHandler.GetTemplate(templateId);

            if (template is null)
            {
                return NotFound();
            }

            return Ok(template);
        }

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <returns>All templates.</returns>
        /// <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public async Task<ActionResult> GetAll()
        {
            var templates = await _futureNhsTemplateHandler.GetAllTemplates();

            if (templates is null | !templates.Any())
            {
                return NotFound();
            }

            return Ok(templates);
        }

        /// <summary>
        /// Deletes the specified template.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns>The template identifier.</returns>
        [HttpDelete("{templateId:guid}")]
        public virtual async Task<ActionResult> Delete(Guid templateId)
        {
            var result = await _futureNhsContentHandler.DeleteContent(templateId);

            if (result)
            {
                return Ok("Successfully deleted: " + templateId);
            }

            return Problem("Deletion unsuccessful: " + templateId);
        }
    }
}