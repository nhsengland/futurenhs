using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    /// <summary>
    /// FutureNhsSitemapHandler to access FutureNhsSitemapService class.
    /// </summary>
    /// <seealso cref="IFutureNhsSiteMapHandler" />
    public class FutureNhsSiteMapHandler : IFutureNhsSiteMapHandler
    {
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsSiteMapService _futureNhsSiteMapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureNhsSiteMapHandler"/> class.
        /// </summary>
        /// <param name="futureNhsContentService">The future NHS content service.</param>
        /// <param name="futureNhsSiteMapService">The future NHS site map service.</param>
        public FutureNhsSiteMapHandler(IFutureNhsContentService futureNhsContentService, IFutureNhsSiteMapService futureNhsSiteMapService)
        {
            _futureNhsContentService = futureNhsContentService;
            _futureNhsSiteMapService = futureNhsSiteMapService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<GroupSitemapItemViewModel>> GetGroupSitemapItemsAsync(Guid pageId)
        {
            // Get published page
            var page = await _futureNhsContentService.GetPublishedAsync(pageId);

            // If it doesn't exist, return as empty.
            if (page is null)
            {
                return Enumerable.Empty<GroupSitemapItemViewModel>();
            }
            else
            {
                // if page is root node, generate tree from this page.
                if (_futureNhsSiteMapService.IsRoot(page))
                {
                    return PopulateGroupSiteMapItemViewModel(page);
                }
                // else, find root and generate from that page.
                else
                {
                    return PopulateGroupSiteMapItemViewModel(_futureNhsSiteMapService.GetRoot(page));
                }
            }
        }

        /// <summary>
        /// Populates the group site map item view model.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>GroupSiteMapItemViewModel.</returns>
        private IEnumerable<GroupSitemapItemViewModel> PopulateGroupSiteMapItemViewModel(IPublishedContent root)
        {
            var list = new List<GroupSitemapItemViewModel>();
            var descendants = root.Descendants().Where(x => x.IsPublished());

            foreach (var item in descendants)
            {
                yield return new GroupSitemapItemViewModel()
                {
                    Name = item.Name,
                    Title = item.Value("title", fallback: Fallback.ToDefaultValue, defaultValue: "No title."),
                    Id = item.Key,
                    ParentId = item.Parent.Key,
                    Url = item.Url(),
                    Level = item.Level
                };
            }
        }
    }
}
