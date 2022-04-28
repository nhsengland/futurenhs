using FutureNHS.Api.Models.Content;
using FutureNHS.Api.Models.Content.Requests;

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
        Task<ApiResponse<string>> CreateContentAsync(Guid? userId, Guid groupId, GeneralWebPageCreateRequest createRequest, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the content asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="pageContent">Content of the page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdateContentAsync(Guid? userId, Guid contentId, PageContentModel pageContent, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes the content asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> DeleteContentAsync(Guid? userId, Guid contentId, int? contentLevel, CancellationToken cancellationToken);
        /// <summary>
        /// Publishes the content asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> PublishContentAsync(Guid contentId, CancellationToken cancellationToken);
    }
}
