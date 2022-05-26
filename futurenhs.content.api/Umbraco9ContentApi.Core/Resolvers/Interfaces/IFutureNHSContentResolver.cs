using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using ContentModelData = Umbraco9ContentApi.Core.Models.Content.ContentModelData;

namespace Umbraco9ContentApi.Core.Resolvers.Interfaces
{
    public interface IFutureNhsContentResolver
    {
        ContentModelData ResolveContent(IPublishedElement content, string propertyGroupAlias = "content", Dictionary<string, object>? options = null);
        ContentModelData ResolveContent(IContent content, string propertyGroupAlias = "content", Dictionary<string, object>? options = null);
    }
}
