namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsContentHandler
    {
        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageParentId">The page parent identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreateContentAsync(string pageName, string pageParentId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the content asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="pageContent">The content.</param>
        /// <returns>True or false.</returns>
        Task<ApiResponse<string>> UpdateContentAsync(Guid id, string title, string description, PageContentModel pageContent, CancellationToken cancellationToken);

        /// <summary>
        /// Publishes the content asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> PublishContentAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the content asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> DeleteContentAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the content published asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetContentPublishedAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the content draft asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetContentDraftAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all content asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllContentAsync(CancellationToken cancellationToken);
    }
}
