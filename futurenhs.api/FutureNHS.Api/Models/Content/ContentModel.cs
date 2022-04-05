using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content
{
    public class ContentModel
    {
        [JsonProperty("system")]
        public SystemModel? System { get; set; }

        [JsonProperty("fields")]
        public Dictionary<string, object>? Fields { get; set; }
    }
}
