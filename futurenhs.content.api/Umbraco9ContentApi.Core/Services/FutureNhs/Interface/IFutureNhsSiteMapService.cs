using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsSiteMapService
    {
        bool IsRoot(IPublishedContent pageId);
        IPublishedContent GetRoot(IPublishedContent page);
    }
}
