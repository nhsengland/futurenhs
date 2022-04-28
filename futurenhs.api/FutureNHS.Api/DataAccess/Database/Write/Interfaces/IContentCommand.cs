using FutureNHS.Api.Models.Content;
using FutureNHS.Api.Models.Content.Requests;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface IContentCommand
    {
        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreateContentAsync(GeneralWebPageCreateRequest createRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="createRequest">The create request.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdateContentAsync(Guid contentId, PageContentModel createRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> DeleteContentAsync(Guid contentId, CancellationToken cancellationToken);
        /// <summary>
        /// Publishes the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> PublishContentAsync(Guid contentId, CancellationToken cancellationToken);
    }
}
