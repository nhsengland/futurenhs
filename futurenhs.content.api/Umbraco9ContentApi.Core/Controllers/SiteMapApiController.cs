using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
using Umbraco9ContentApi.Core.Models;

namespace Umbraco9ContentApi.Core.Controllers
{
    /// <summary>
    /// Sitemap Api Controller.
    /// </summary>
    /// <seealso cref="UmbracoApiController" />
    [Route("api/sitemap")]
    public sealed class SitemapApiController : UmbracoApiController
    {
        IFutureNhsSiteMapHandler _futureNhsSiteMapHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapApiController"/> class.
        /// </summary>
        /// <param name="futureNhsSiteMapHandler">The future NHS site map handler.</param>
        public SitemapApiController(IFutureNhsSiteMapHandler futureNhsSiteMapHandler)
        {
            _futureNhsSiteMapHandler = futureNhsSiteMapHandler;
        }

        /// <summary>
        /// Gets the specified page identifier.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        [HttpGet("{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SitemapGroupItemModel>))]
        public async Task<ActionResult> GetSiteMapAsync(Guid pageId)
        {
            var response = await _futureNhsSiteMapHandler.GetSitemapGroupItemsAsync(pageId);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem(response.Message);
        }
    }
}
