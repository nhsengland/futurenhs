using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
using Umbraco9ContentApi.Core.Models.Sitemap;
using Umbraco9ContentApi.Core.Models.Response;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    /// <summary>
    /// FutureNhsSitemapHandler to access FutureNhsSitemapService class.
    /// </summary>
    /// <seealso cref="IFutureNhsSiteMapHandler" />
    public sealed class FutureNhsSiteMapHandler : IFutureNhsSiteMapHandler
    {
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsSiteMapService _futureNhsSiteMapService;
        private List<string>? errorList = new List<string>();

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
        public async Task<ApiResponse<IEnumerable<SitemapGroupItemModel>>> GetSitemapGroupItemsAsync(Guid pageId, CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<SitemapGroupItemModel>> response = new ApiResponse<IEnumerable<SitemapGroupItemModel>>();

            // Get published page
            var page = await _futureNhsContentService.GetPublishedContentAsync(pageId, cancellationToken);

            // If it doesn't exist, return as empty.
            if (page is null)
            {
                errorList.Add("Couldn't retrieve page.");
                return response.Failure(errorList, "Failed.");
            }

            // if page is root node, generate tree from this page.
            if (_futureNhsSiteMapService.IsRoot(page))
            {
                return response.Success(PopulateGroupSiteMapItemViewModel(page), "Success.");
            }

            // else, find root and generate from that page.
            return response.Success(PopulateGroupSiteMapItemViewModel(_futureNhsSiteMapService.GetRoot(page)), "Success.");

        }

        /// <summary>
        /// Populates the group site map item view model.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>GroupSiteMapItemViewModel.</returns>
        private IEnumerable<SitemapGroupItemModel> PopulateGroupSiteMapItemViewModel(IPublishedContent root)
        {
            var list = new List<SitemapGroupItemModel>();
            var descendants = root.Descendants().Where(x => x.IsPublished());

            foreach (var item in descendants)
            {
                yield return new SitemapGroupItemModel(
                    item.Key,
                    item.Name,
                    item.Value("title", fallback: Fallback.ToDefaultValue, defaultValue: "No title."),
                    item.Parent.Key,
                    item.CreateDate,
                    item.UpdateDate,
                    item.Level
                    );
            }
        }
    }
}
