using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco9ContentApi.Core.Models;

namespace Umbraco9ContentApi.Core.Resolvers.Interfaces
{
    public interface IFutureNHSContentResolver
    {
        ContentModel ResolveContent(IPublishedElement content, Dictionary<string, object>? options = null);
    }
}
