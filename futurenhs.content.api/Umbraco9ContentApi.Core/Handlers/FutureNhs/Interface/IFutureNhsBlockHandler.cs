namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Response;
    using UmbracoContentApi.Core.Models;

    public interface IFutureNhsBlockHandler
    {
        /// <summary>
        /// Gets all blocks.
        /// </summary>
        /// <returns>All blocks.</returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllBlocksAsync();
    }
}