using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Models.Content;
using Umbraco9ContentApi.Core.Models.Response;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    public interface IFutureNhsPageHandler
    {
        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageParentId">The page parent identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<string> CreatePage(string pageName, string pageParentId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets all pages.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<IEnumerable<ContentModel>> GetAllPages(CancellationToken cancellationToken);
        /// <summary>
        /// Updates the content of the user editing.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<string> UpdateUserEditingContent(Guid userId, Guid pageId, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the page.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pageModel">The page model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<string> UpdatePage(Guid id, PageModel pageModel, CancellationToken cancellationToken);
        /// <summary>
        /// Checks the page edit status.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>UserId of the editing user if page has a draft.</returns>
        ApiResponse<string> CheckPageEditStatus(Guid pageId, CancellationToken cancellationToken);
    }
}
