using System.Text.Json.Serialization;

namespace Notify.Models.Responses
{
    public class Link
    {
        [JsonPropertyName("current")]
        public string current;
        [JsonPropertyName("next")]
        public string next;
    }
}
