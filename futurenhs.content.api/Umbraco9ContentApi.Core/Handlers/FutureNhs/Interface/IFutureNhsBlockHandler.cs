namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Response;
    using Umbraco9ContentApi.Core.Models;

    public interface IFutureNhsBlockHandler
    {
        /// <summary>
        /// Gets all blocks asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllBlocksAsync(CancellationToken cancellationToken);
    }
}