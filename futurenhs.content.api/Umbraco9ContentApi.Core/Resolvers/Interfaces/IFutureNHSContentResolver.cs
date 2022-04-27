using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using ContentModel = Umbraco9ContentApi.Core.Models.ContentModel;

namespace Umbraco9ContentApi.Core.Resolvers.Interfaces
{
    public interface IFutureNhsContentResolver
    {
        ContentModel ResolveContent(IPublishedElement content, Dictionary<string, object>? options = null);
        ContentModel ResolveContent(IContent content, Dictionary<string, object>? options = null);
    }
}
