namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using ContentModelData = Models.Content.ContentModelData;

    public interface IFutureNhsContentService
    {
        /// <summary>
        /// Gets the published content.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        IPublishedContent GetPublishedContent(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="contentId">The identifier.</param>
        /// <returns></returns>
        IContent GetDraftContent(Guid contentId, CancellationToken cancellationToken);
        /// <summary>
        /// Resolves the published content.
        /// </summary>
        /// <param name="publishedContent">The content.</param>
        /// <param name="propertyGroupAlias">The property group alias.</param>
        /// <returns></returns>
        ContentModelData ResolvePublishedContent(IPublishedContent publishedContent, string propertyGroupAlias, CancellationToken cancellationToken);
        /// <summary>
        /// Resolves the draft content.
        /// </summary>
        /// <param name="draftContent">The content.</param>
        /// <returns></returns>
        ContentModelData ResolveDraftContent(IContent draftContent, CancellationToken cancellationToken);
        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="documentTypeAlias">The document type alias.</param>
        /// <returns></returns>
        IContent CreateContent(string Name, Guid parentId, string documentTypeAlias, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes the.
        /// </summary>
        /// <param name="contentId">The identifier.</param>
        /// <returns></returns>
        void DeleteContent(Guid contentId, CancellationToken cancellationToken);
        /// <summary>
        /// Publishes the.
        /// </summary>
        /// <param name="content">The identifier.</param>
        /// <returns></returns>
        void PublishContent(IContent content, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the template blocks.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        List<ContentModelData> GetTemplateBlocks(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// Sets the content property value.
        /// </summary>
        /// <param name="content">The content to update.</param>
        /// <param name="propertAlias">The propert alias.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        IContent SetContentPropertyValue(IContent content, string propertAlias, object value, CancellationToken cancellationToken);
        /// <summary>
        /// Saves the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        void SaveContent(IContent content, CancellationToken cancellationToken);
        /// <summary>
        /// Saves the and publish content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        void SaveAndPublishContent(IContent content, CancellationToken cancellationToken);
        /// <summary>
        /// Assigns the block to the content.
        /// </summary>
        /// <param name="parentContent">The parent content.</param>
        /// <param name="blockId">The block identifier.</param>
        /// <returns></returns>
        IContent AssignBlockToContent(IContent parentContent, Guid blockId, CancellationToken cancellationToken);
        /// <summary>
        /// Gets the associated blocks.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        List<IPublishedContent> GetAssociatedPublishedContentBlocks(IPublishedContent content, CancellationToken cancellationToken);
        /// <summary>
        /// Rollbacks the draft content.
        /// </summary>
        /// <param name="draftContent">Content of the draft.</param>
        /// <returns></returns>
        void RollbackDraftContent(IContent draftContent, CancellationToken cancellationToken);
        /// <summary>
        /// Compares the content model lists.
        /// </summary>
        /// <param name="contentModelList">The content model list.</param>
        /// <param name="comparedcontentModelList">The comparedcontent model list.</param>
        /// <returns></returns>
        IEnumerable<Guid> CompareContentModelLists(IEnumerable<ContentModelData> contentModelList, IEnumerable<ContentModelData> comparedcontentModelList);
    }
}
