using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content
{
    public class ItemModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("contentType")]
        public string? ContentType { get; set; }
    }
}
