using Umbraco.Cms.Core.Models;

namespace Umbraco9ContentApi.Core.Resolvers.Interface
{
    public interface IDraftContentResolver
    {
        Models.ContentModel ResolveContent(IContent content, Dictionary<string, object>? options = null);
    }
}
