using System.Text.Json.Serialization;

namespace Umbraco9ContentApi.Core.Models.Content
{
    public class ContentModelItem
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public string? UrlSegment { get; set; }
        [JsonIgnore]
        public string? Type { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public DateTime EditedAt { get; set; }
        public string? ContentType { get; set; }
        [JsonIgnore]
        public string? Locale { get; set; }
        [JsonIgnore]
        public string? Url { get; set; }
    }
}
