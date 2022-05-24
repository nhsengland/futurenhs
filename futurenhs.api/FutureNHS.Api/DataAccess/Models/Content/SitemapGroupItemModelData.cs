using Newtonsoft.Json;

namespace FutureNHS.Api.DataAccess.Models.Content
{
    public sealed class SitemapGroupItemModelData
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        [JsonProperty("parentId")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
