using Umbraco9ContentApi.Core.Models.Sitemap;
using Umbraco9ContentApi.Core.Models.Response;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    /// <summary>
    /// FutureNhsSitemapHandler interface.
    /// </summary>
    public interface IFutureNhsSiteMapHandler
    {
        /// <summary>
        /// Gets the sitemap group items.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<IEnumerable<SitemapGroupItemModel>> GetSitemapGroupItems(Guid pageId, CancellationToken cancellationToken);
    }
}
