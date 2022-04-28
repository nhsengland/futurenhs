using Newtonsoft.Json;
using Umbraco9ContentApi.Core.Models.Content;

namespace Umbraco9ContentApi.Core.Models
{
    public class PageContentModel
    {
        [JsonProperty("blocks")]
        public ContentModel[] Blocks { get; set; }
    }
}
