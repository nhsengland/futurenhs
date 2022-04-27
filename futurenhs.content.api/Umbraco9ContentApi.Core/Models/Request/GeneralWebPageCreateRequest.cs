using Newtonsoft.Json;

namespace Umbraco9ContentApi.Core.Models.Requests
{
    public class GeneralWebPageCreateRequest
    {
        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        /// <value>
        /// The name of the page.
        /// </value>
        [JsonProperty("pageName")]
        public string PageName { get; set; }

        /// <summary>
        /// Gets or sets the page parent identifier.
        /// </summary>
        /// <value>
        /// The page parent identifier.
        /// </value>
        [JsonProperty("pageParentId")]
        public string PageParentId { get; set; }
    }
}