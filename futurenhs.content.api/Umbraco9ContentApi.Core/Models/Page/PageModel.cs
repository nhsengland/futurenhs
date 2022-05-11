using Newtonsoft.Json;
using Umbraco9ContentApi.Core.Models.Content;

namespace Umbraco9ContentApi.Core.Models
{
    public class PageModel
    {
        [JsonProperty("blocks")]
        public ContentModel[] Blocks { get; set; }
    }
}
