using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;

namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    public sealed class FutureNhsSiteMapService : IFutureNhsSiteMapService
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly ILogger<FutureNhsSiteMapService> _logger;


        public FutureNhsSiteMapService(IFutureNhsContentService futureNhsContentService, IConfiguration config, ILogger<FutureNhsSiteMapService> logger)
        {
            _futureNhsContentService = futureNhsContentService;
            _config = config;
            _logger = logger;
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
            // be the root/homepage node for the group.
            return page.Ancestors().Where(x => x.Level == 3).FirstOrDefault();
        }
    }
}
