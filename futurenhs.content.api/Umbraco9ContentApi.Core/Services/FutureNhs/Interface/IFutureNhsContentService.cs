namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using ContentModel = Models.Content.ContentModel;

    /// <summary>
    /// Exposes and extends the Umbraco API.
    /// </summary>
    public interface IFutureNhsContentService
    {
        /// <summary>
        /// Gets the published children.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Published contents children.</returns>
        Task<IEnumerable<IPublishedContent>> GetPublishedChildrenAsync(Guid id);

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>Published content.</returns>
        Task<IPublishedContent> GetPublishedAsync(Guid id);

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<IContent> GetAsync(Guid id);

        /// <summary>
        /// Resolves the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>Content model.</returns>
        Task<ContentModel> ResolveAsync(IPublishedContent content);

        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="parentContentId">The parent content identifier.</param>
        /// <param name="contentName">Name of the content.</param>
        /// <param name="documentTypeAlias">The document type alias.</param>
        /// <returns></returns>
        Task<IContent?> CreateAsync(Guid parentContentId, string contentName, string documentTypeAlias);

        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Operation result.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Publishes the content.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>Publish result.</returns>
        Task<bool> PublishAsync(Guid contentId);

        /// <summary>
        /// Gets the  specified template's blocks.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Blocks for specified template.</returns>
        Task<IEnumerable<ContentModel>> GetTemplateBlocksAsync(Guid id);

        /// <summary>
        /// Saves the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        Task<bool> SaveAsync(IContent content);

        /// <summary>
        /// Saves the content and publish.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        Task<bool> SaveAndPublishAsync(IContent content);
    }
}
