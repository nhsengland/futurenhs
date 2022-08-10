using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Content.Requests;
using FutureNHS.Api.DataAccess.Models.Content.Responses;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface IContentCommand
    {
        /// <summary>
        /// Creates the page asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreatePageAsync(PageDto content, CancellationToken cancellationToken);
        /// <summary>
        /// Creates the block asynchronous.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreateBlockAsync(BlockCreateRequest createRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="pageContent">Content of the page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdatePageAsync(Guid contentId, GeneralWebPageUpdateRequest pageContent, CancellationToken cancellationToken);
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
        /// <summary>
        /// Discards the content of the draft.
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
