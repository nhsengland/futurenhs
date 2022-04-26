namespace Umbraco9ContentApi.Core.Models.Request
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The request used to update the content for a web page.
    /// </summary>
    public class GeneralWebPageUpdateRequest
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [ModelBinder(Name = "title")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [ModelBinder(Name = "description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [ModelBinder(Name = "pageContent")]
        public PageContentModel? PageContent { get; set; }
    }
}