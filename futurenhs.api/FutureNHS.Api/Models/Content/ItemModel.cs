using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content
{
    public class ItemModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("urlSegment")]
        public string? UrlSegment { get; set; }
        [JsonProperty("type")]
        public string? Type { get; set; }
        [JsonProperty("createAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("editAt")]
        public DateTime EditedAt { get; set; }
        [JsonProperty("contentType")]
        public string? ContentType { get; set; }
        [JsonProperty("locale")]
        public string? Locale { get; set; }
        [JsonProperty("url")]
        public string? Url { get; set; }
    }
}
