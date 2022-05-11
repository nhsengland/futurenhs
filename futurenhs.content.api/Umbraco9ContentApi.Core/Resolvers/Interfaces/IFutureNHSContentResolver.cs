using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using ContentModel = Umbraco9ContentApi.Core.Models.Content.ContentModel;

namespace Umbraco9ContentApi.Core.Resolvers.Interfaces
{
    public interface IFutureNhsContentResolver
    {
        ContentModel ResolveContent(IPublishedElement content, string propertyGroupAlias = "content", Dictionary<string, object>? options = null);
        ContentModel ResolveContent(IContent content, string propertyGroupAlias = "content", Dictionary<string, object>? options = null);
    }
}
