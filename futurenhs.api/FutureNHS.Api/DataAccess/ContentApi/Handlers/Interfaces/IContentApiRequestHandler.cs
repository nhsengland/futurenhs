using FutureNHS.Api.Models.Content;

namespace FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces
{
    public interface IContentApiRequestHandler
    {
        /// <summary>
        /// Gets the content published asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetContentPublishedAsnyc(Guid contentId);
        /// <summary>
        /// Gets the content draft asnyc.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetContentDraftAsnyc(Guid contentId);
        /// <summary>
        /// Gets the blocks asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetBlocksAsync();
        /// <summary>
        /// Gets the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetBlockAsync(Guid blockId);
        /// <summary>
        /// Gets the block placeholder values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockPlaceholderValuesAsync(Guid blockId);
        /// <summary>
        /// Gets the block field values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockFieldValuesAsync(Guid blockId);
        /// <summary>
        /// Gets the template asynchronous.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModel>> GetTemplateAsync(Guid templateId);
        /// <summary>
        /// Gets the templates asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetTemplatesAsync();
        /// <summary>
        /// Gets the site map asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<SitemapGroupItemModel>>> GetSiteMapAsync(Guid contentId);
    }
}
