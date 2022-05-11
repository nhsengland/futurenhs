namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using ContentModel = Models.Content.ContentModel;

    public interface IFutureNhsContentService
    {
        /// <summary>
        /// Gets the published content children asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<IPublishedContent>?> GetPublishedContentChildrenAsync(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the published content asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IPublishedContent> GetPublishedContentAsync(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the content asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IContent> GetDraftContentAsync(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Resolves the published content asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="propertyGroupAlias">The property group alias.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ContentModel> ResolvePublishedContentAsync(IPublishedContent content, string propertyGroupAlias, CancellationToken cancellationToken);
        /// <summary>
        /// Resolves the draft content asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ContentModel> ResolveDraftContentAsync(IContent content, CancellationToken cancellationToken);
        /// <summary>
        /// Creates the content asynchronous.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="documentTypeAlias">The document type alias.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IContent> CreateContentAsync(string Name, Guid parentId, string documentTypeAlias, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> DeleteContentAsync(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes the content of the associated.
        /// </summary>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> DeleteAssociatedContent(Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> PublishContentAsync(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Updates the user editing content asynchronous.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the template blocks asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<ContentModel>> GetTemplateBlocksAsync(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Saves the content asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> SaveContentAsync(IContent content, CancellationToken cancellationToken);
        /// <summary>
        /// Saves the and publish content asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> SaveAndPublishContentAsync(IContent content, CancellationToken cancellationToken);
        /// <summary>
        /// Assigns the block to the content.
        /// </summary>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="blockId">The block identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> AssignBlockToContent(Guid parentId, Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the associated blocks.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        List<IPublishedContent> GetAssociatedPublishedBlocks(IPublishedContent content);
        /// <summary>
        /// Rollbacks the draft content asynchronous.
        /// </summary>
        /// <param name="draftContent">Content of the draft.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        bool RollbackDraftContentAsync(IContent draftContent, CancellationToken cancellationToken);
    }
}
