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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GroupSitemapItemViewModel>))]
        public async Task<ActionResult> GetAsync(Guid pageId)
        {
            var siteMapItems = await _futureNhsSiteMapHandler.GetGroupSitemapItemsAsync(pageId);

            if (siteMapItems is null || !siteMapItems.Any())
            {
                return NotFound();
            }

            return Ok(siteMapItems);
        }
    }
}
