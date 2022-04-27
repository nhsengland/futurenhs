using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Models.Response;

namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsTemplateHandler
    {
        /// <summary>
        /// Gets the template asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetTemplateAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all templates asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllTemplatesAsync(CancellationToken cancellationToken);
    }
}
