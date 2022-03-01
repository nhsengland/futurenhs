namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;

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
        Task<IEnumerable<IPublishedContent>> GetPublishedChildren(Guid id);

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>Published content.</returns>
        Task<IPublishedContent> GetPublished(Guid id);

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<IContent> Get(Guid id);

        /// <summary>
        /// Resolves the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>Content model.</returns>
        Task<ContentModel> Resolve(IPublishedContent content);

        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="parentContentId">The parent content identifier.</param>
        /// <param name="contentName">Name of the content.</param>
        /// <param name="documentTypeAlias">The document type alias.</param>
        /// <returns></returns>
        Task<IContent?> Create(Guid parentContentId, string contentName, string documentTypeAlias);

        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Operation result.</returns>
        Task<bool> Delete(Guid id);

        /// <summary>
        /// Publishes the content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>PPublish result.</returns>
        Task<bool> Publish(Guid id);

        /// <summary>
        /// Gets the  specified template's blocks.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Blocks for specified template.</returns>
        Task<IEnumerable<ContentModel>> GetTemplateBlocks(Guid id);

        /// <summary>
        /// Saves the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        Task<bool> Save(IContent content);

        /// <summary>
        /// Saves the content and publish.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        Task<bool> SaveAndPublish(IContent content);
    }
}
