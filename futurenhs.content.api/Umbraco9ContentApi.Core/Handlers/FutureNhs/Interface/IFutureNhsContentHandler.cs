namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsContentHandler
    {
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
        Task<ApiResponse<ContentModel>> GetPublishedContentAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the content draft asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetDraftContentAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all content asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllPagesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Discards the draft content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> DiscardDraftContentAsync(Guid contentId, CancellationToken cancellationToken);
    }
}
