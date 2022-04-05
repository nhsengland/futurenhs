using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content.Requests
{

    /// <summary>
    /// The request used to create a new page via the Content Api.
    /// </summary>
    public class GeneralWebPageCreateRequest
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
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        [JsonProperty("parentId")]
        public string? ParentId { get; set; }
    }
}