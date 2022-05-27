using Newtonsoft.Json;

namespace FutureNHS.Api.DataAccess.Models.Content.Requests
{

    /// <summary>
    /// The request used to create a new page via the Content Api.
    /// </summary>
    public class GeneralWebPageCreateRequest
    {
        /// <summary>
        /// Gets or sets the page parent identifier.
        /// </summary>
        /// <value>
        /// The page parent identifier.
        /// </value>
        [JsonProperty("pageParentId")]
        public Guid? PageParentId { get; set; }
    }
}