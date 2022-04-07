using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Group.Requests
{
    public class GroupCreateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("strapline")]
        public string Strapline { get; set; }

        [JsonProperty("imageId")]
        public Guid ImageId { get; set; }

        [JsonProperty("groupOwner")]
        public Guid GroupOwner { get; set; }

        [JsonProperty("groupAdministrators")]
        public Guid[] GroupAdministrators { get; set; }
    }
}
