using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Content.Responses;
using FutureNHS.Api.DataAccess.Models.Requests;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface IContentCommand
    {
        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreateContentAsync(ContentDto content, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="updateRequest">The update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdateContentAsync(Guid contentId, GeneralWebPageUpdateRequest updateRequest, CancellationToken cancellationToken);
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
