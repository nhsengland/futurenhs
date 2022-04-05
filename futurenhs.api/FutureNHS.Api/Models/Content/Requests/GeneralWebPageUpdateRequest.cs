using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content.Requests
{

    /// <summary>
    /// The request used to update a page via the Content Api.
    /// </summary>
    public class GeneralWebPageUpdateRequest
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonProperty("pageContent")]
        public string? PageContent { get; set; }
    }
}