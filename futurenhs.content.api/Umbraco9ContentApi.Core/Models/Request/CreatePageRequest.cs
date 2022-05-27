using Newtonsoft.Json;

namespace Umbraco9ContentApi.Core.Models.Requests
{
    public class CreatePageRequest
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("pageName")]
        public string PageName { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        [JsonProperty("pageParentId")]
        public string PageParentId { get; set; }
    }
}