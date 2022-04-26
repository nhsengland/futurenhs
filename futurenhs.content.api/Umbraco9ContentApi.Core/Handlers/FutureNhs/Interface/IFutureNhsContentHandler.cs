namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsContentHandler
    {
        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageParentId">The page parent identifier.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns>Created content identifier.</returns>
        Task<ApiResponse<string>> CreateContentAsync(string pageName, string? pageParentId = null, bool publish = false);

        /// <summary>
        /// Updates the content of the page.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="pageContent">The content.</param>
        /// <returns>True or false.</returns>
        Task<ApiResponse<string>> UpdateContentAsync(Guid id, string? title, string? description, PageContentModel? pageContent);

        /// <summary>
        /// Publishes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>True or false.</returns>
        Task<ApiResponse<string>> PublishContentAsync(Guid id);

        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>True or false.</returns>
        Task<ApiResponse<string>> DeleteContentAsync(Guid id);

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Specified content.</returns>
        Task<ApiResponse<ContentModel>> GetContentAsync(Guid id);

        /// <summary>
        /// Gets all content.
        /// </summary>
        /// <returns>All content.</returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllContentAsync();
    }
}
