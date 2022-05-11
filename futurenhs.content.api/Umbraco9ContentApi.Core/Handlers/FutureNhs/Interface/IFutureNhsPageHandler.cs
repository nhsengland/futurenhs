using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Models.Response;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    public interface IFutureNhsPageHandler
    {
        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageParentId">The page parent identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> CreatePageAsync(string pageName, string pageParentId, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the user editing content asynchronous.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the page asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pageModel">The page model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpdatePageAsync(Guid id, PageModel pageModel, CancellationToken cancellationToken);
        /// <summary>
        /// Checks the page edit status asynchronous.
        /// </summary>
        /// <param name="pageId">The page unique identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>UserId of the editing user if page has a draft.</returns>
        Task<ApiResponse<string>> CheckPageEditStatusAsync(Guid pageId, CancellationToken cancellationToken);
    }
}
