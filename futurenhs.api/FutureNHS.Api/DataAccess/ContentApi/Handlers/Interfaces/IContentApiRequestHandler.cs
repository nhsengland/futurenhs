using FutureNHS.Api.DataAccess.Models.Content;
using FutureNHS.Api.DataAccess.Models.Content.Responses;

namespace FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces
{
    public interface IContentApiRequestHandler
    {
        /// <summary>
        /// Gets the content published asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModelData>> GetContentPublishedAsnyc(Guid contentId);
        /// <summary>
        /// Gets the content draft asnyc.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModelData>> GetContentDraftAsnyc(Guid contentId);
        /// <summary>
        /// Gets the blocks asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModelData>>> GetAllBlocksAsync();
        /// <summary>
        /// Gets the block asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModelData>> GetBlockAsync(Guid blockId);
        /// <summary>
        /// Gets the block fields placeholder values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockContentPlaceholderValuesAsync(Guid blockId);
        /// <summary>
        /// Gets the block labels placeholder values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockLabelsPlaceholderValuesAsync(Guid blockId);
        /// <summary>
        /// Gets the block field values asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockContentAsync(Guid blockId);
        /// <summary>
        /// Gets the block labels asynchronous.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetBlockLabelsAsync(Guid blockId);
        /// <summary>
        /// Gets the template asynchronous.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<ContentModelData>> GetTemplateAsync(Guid templateId);
        /// <summary>
        /// Gets the templates asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ContentModelData>>> GetTemplatesAsync();
        /// <summary>
        /// Gets the site map asynchronous.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<SitemapGroupItemModelData>>> GetSiteMapAsync(Guid contentId);
    }
}
