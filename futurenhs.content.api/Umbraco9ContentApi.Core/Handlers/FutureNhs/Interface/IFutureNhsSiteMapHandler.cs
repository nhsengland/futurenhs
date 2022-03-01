using Umbraco9ContentApi.Core.Models;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    /// <summary>
    /// FutureNhsSitemapHandler interface.
    /// </summary>
    public interface IFutureNhsSiteMapHandler
    {
        /// <summary>
        /// Gets the group site map items.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <returns>Group sitemap items.</returns>
        Task<IEnumerable<GroupSitemapItemViewModel>> GetGroupSitemapItems(Guid pageId);
    }
}
