namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco.Cms.Core.Models;
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;

    public interface IFutureNhsContentHandler
    {
        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageParentId">The page parent identifier.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns>Created content.</returns>
        Task<IContent> CreateContentAsync(string pageName, string? pageParentId = null, bool publish = false);

        /// <summary>
        /// Updates the content of the page.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="pageContent">The content.</param>
        /// <returns>True or false.</returns>
        Task<bool> UpdateContentAsync(Guid id, string title, string description, string pageContent);

        /// <summary>
        /// Publishes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>True or false.</returns>
        Task<bool> PublishContentAsync(Guid id);

        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>True or false.</returns>
        Task<bool> DeleteContentAsync(Guid id);

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Specified content.</returns>
        Task<ContentModel> GetContentAsync(Guid id);

        /// <summary>
        /// Gets all content.
        /// </summary>
        /// <returns>All content.</returns>
        Task<IEnumerable<ContentModel>> GetAllContentAsync();
    }
}
