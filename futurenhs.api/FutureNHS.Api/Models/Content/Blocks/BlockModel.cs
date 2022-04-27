using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content.Blocks
{
    public class BlockModel
    {
        [JsonProperty("item")]
        public ItemModel Item { get; set; }
        [JsonProperty("fields")]
        public List<string> Fields { get; set; }
    }
}
