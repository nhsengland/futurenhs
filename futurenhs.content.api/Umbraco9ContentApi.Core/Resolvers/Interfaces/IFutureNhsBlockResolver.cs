using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco9ContentApi.Core.Models.Blocks;

namespace Umbraco9ContentApi.Core.Resolvers.Interfaces
{
    public interface IFutureNhsBlockResolver
    {
        BlockModel ResolveContent(IPublishedElement content, Dictionary<string, object>? options = null);
    }
}
