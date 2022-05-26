using FutureNHS.Api.DataAccess.Models.Content.Requests;
using FutureNHS.Api.DataAccess.Models.Content.Responses;


namespace FutureNHS.Api.Services.Interfaces
{
    public interface IContentService
    {
        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreatePageAsync(Guid userId, Guid groupId, GeneralWebPageCreateRequest? createRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Creates the block asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreateBlockAsync(Guid userId, BlockCreateRequest createRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the page asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="updateRequest">The update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdatePageAsync(Guid userId, Guid contentId, GeneralWebPageUpdateRequest updateRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes the content asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> DeleteContentAsync(Guid userId, Guid contentId, int? contentLevel, CancellationToken cancellationToken);
        /// <summary>
        /// Publishes the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> PublishContentAsync(Guid contentId, CancellationToken cancellationToken);
        /// <summary>
        /// Discards the draft content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> DiscardDraftContentAsync(Guid contentId, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the user editing content asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken);
        /// <summary>
        /// Checks the page edit status asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CheckPageEditStatusAsync(Guid userId, Guid pageId, CancellationToken cancellationToken);
    }
}
