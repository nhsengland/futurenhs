using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
using Umbraco9ContentApi.Core.Models.Sitemap;

namespace Umbraco9ContentApi.Core.Controllers
{
    /// <summary>
    /// Sitemap Api Controller.
    /// </summary>
    /// <seealso cref="UmbracoApiController" />
    [Route("api/sitemap")]
    public sealed class SitemapController : UmbracoApiController
    {
        IFutureNhsSiteMapHandler _futureNhsSiteMapHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapController"/> class.
        /// </summary>
        /// <param name="futureNhsSiteMapHandler">The future NHS site map handler.</param>
        public SitemapController(IFutureNhsSiteMapHandler futureNhsSiteMapHandler)
        {
            _futureNhsSiteMapHandler = futureNhsSiteMapHandler;
        }

        /// <summary>
        /// Gets the site map asynchronous.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SitemapGroupItemModel>))]
        public async Task<ActionResult> GetSiteMapAsync(Guid pageId, CancellationToken cancellationToken)
        {
            var response = await _futureNhsSiteMapHandler.GetSitemapGroupItemsAsync(pageId, cancellationToken);

            if (response.Succeeded)
            {
                return Ok(response);
            }

            return Problem(response.Message);
        }
    }
}
