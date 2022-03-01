using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;

namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    public class FutureNhsSiteMapService : IFutureNhsSiteMapService
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;

        public FutureNhsSiteMapService(IFutureNhsContentService futureNhsContentService, IConfiguration config)
        {
            _futureNhsContentService = futureNhsContentService;
            _config = config;
        }

        public bool IsRoot(IPublishedContent page)
        {
            // store in config?
            var rootNodeGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");

            if (page.Parent.Key == rootNodeGuid)
            {
                return true;
            }

            return false;
        }

        public IPublishedContent GetRoot(IPublishedContent page)
        {
            // Level 2 is the Groups folder so we seek level 3 which would
            // be the root node for the current group.
            return page.Ancestors().Where(x => x.Level == 3).FirstOrDefault();
        }
    }
}
