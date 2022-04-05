using UmbracoContentApi.Core.Models;

namespace Umbraco9ContentApi.Core.Models
{
    public class PageContentModelData
    {
        public SystemModel? System { get; set; }
        public ContentModel[] Fields { get; set; }
    }
}
