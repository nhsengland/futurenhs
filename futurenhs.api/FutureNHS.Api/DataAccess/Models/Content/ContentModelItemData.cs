using Newtonsoft.Json;

namespace FutureNHS.Api.DataAccess.Models.Content
{
    public sealed class ContentModelItemData
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("editedAt")]
        public string? EditedAt { get; set; }
        [JsonProperty("contentType")]
        public string? ContentType { get; set; }
    }
}
