using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;

namespace Umbraco9ContentApi.Core.Controllers.Base
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FutureNhsBaseController : UmbracoApiController
    {
        private readonly IFutureNhsContentHandler _futureNhsContentHandler;

        public FutureNhsBaseController(IFutureNhsContentHandler futureNhsContentHandler)
        {
            _futureNhsContentHandler = futureNhsContentHandler;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The content.</returns>
        [HttpGet("{id:guid}")]
        public virtual async Task<ActionResult> Get(Guid id)
        {
            var content = await _futureNhsContentHandler.GetContent(id);

            if (content is null)
            {
                return NoContent();
            }

            return Ok(content);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Delete operation result.</returns>
        [HttpDelete("{id:guid}")]
        public virtual async Task<ActionResult> Delete(Guid id)
        {
            var result = await _futureNhsContentHandler.DeleteContent(id);

            if (result)
            {
                return Ok("Successfully deleted: " + id);
            }

            return Problem("Deletion unsuccessful: " + id);
        }
    }
}
