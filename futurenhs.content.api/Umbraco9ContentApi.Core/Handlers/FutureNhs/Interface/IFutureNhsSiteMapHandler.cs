using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Models.Response;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    /// <summary>
    /// FutureNhsSitemapHandler interface.
    /// </summary>
    public interface IFutureNhsSiteMapHandler
    {
        /// <summary>
        /// Gets the sitemap group items asynchronous.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<SitemapGroupItemModel>>> GetSitemapGroupItemsAsync(Guid pageId);
    }
}
