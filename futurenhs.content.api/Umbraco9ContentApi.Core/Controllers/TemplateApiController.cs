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
    public sealed class TemplateApiController : UmbracoApiController
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
        /// Gets the template asynchronous.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        [HttpGet("{templateId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentModel))]
        public async Task<ActionResult> GetTemplateAsync(Guid templateId)
        {
            var template = await _futureNhsTemplateHandler.GetTemplateAsync(templateId);

            if (template is null)
            {
                return NotFound();
            }

            return Ok(template);
        }

        /// <summary>
        /// Gets all templates asynchronous.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContentModel>))]
        public async Task<ActionResult> GetAllTemplatesAsync()
        {
            var response = await _futureNhsTemplateHandler.GetAllTemplatesAsync();

            if (response.Succeeded && !response.Data.Any())
            {
                return NotFound("No templates found.");
            }

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem(response.Message);
        }

        /// <summary>
        /// Deletes the template asynchronous.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        [HttpDelete("{templateId:guid}")]
        public async Task<ActionResult> DeleteTemplateAsync(Guid templateId)
        {
            var response = await _futureNhsContentHandler.DeleteContentAsync(templateId);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem("Deletion unsuccessful: " + templateId);
        }
    }
}