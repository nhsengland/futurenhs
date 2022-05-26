namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsContentHandler
    {
        /// <summary>
        /// Publishes the content of the content and associated.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<string> PublishContentAndAssociatedContent(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<string> DeleteContent(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the content published.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<ContentModelData> GetPublishedContent(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the content draft.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<ContentModelData> GetDraftContent(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Discards the draft content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<string> DiscardDraftContent(Guid contentId, CancellationToken cancellationToken);
    }
}
