using Microsoft.AspNetCore.Mvc;

namespace Umbraco9ContentApi.Core.Models.Request
{
    /// <summary>
    /// The request used to create the content for a web page.
    /// </summary>
    public class GeneralWebPageCreateRequest
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [ModelBinder(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        [ModelBinder(Name = "parentId")]
        public string? ParentId { get; set; }
    }
}