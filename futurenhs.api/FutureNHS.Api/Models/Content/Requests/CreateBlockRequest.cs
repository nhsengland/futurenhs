using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content.Requests
{
    public class CreateBlockRequest
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        /// <summary>
        /// Gets or sets the page identifier.
        /// </summary>
        /// <value>
        /// The page identifier.
        /// </value>
        [JsonProperty("parentId")]
        public Guid ParentId { get; set; }
    }
}