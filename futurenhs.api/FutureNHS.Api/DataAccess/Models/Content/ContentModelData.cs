using Newtonsoft.Json;

namespace FutureNHS.Api.DataAccess.Models.Content
{
    public sealed class ContentModelData
    {
        [JsonProperty("item")]
        public ContentModelItemData? Item { get; set; }
        [JsonProperty("content")]
        public Dictionary<string, object>? Content { get; set; }
    }
}
