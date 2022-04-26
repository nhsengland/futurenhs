namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco9ContentApi.Core.Models.Dto;
    using ContentModel = Models.ContentModel;

    public interface IFutureNhsContentService
    {
        /// <summary>
        /// Gets the published content children asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<IPublishedContent>> GetPublishedContentChildrenAsync(Guid id, CancellationToken cancellationToken);

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
        Task<IContent> GetContentAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Resolves the published content asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ContentModel> ResolvePublishedContentAsync(IPublishedContent content, CancellationToken cancellationToken);

        /// <summary>
        /// Resolves the draft content asynchronous.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ContentModel> ResolveDraftContentAsync(IContent content, CancellationToken cancellationToken);

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="generalWebPage">The general web page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IContent> CreateContentAsync(GeneralWebPageDto generalWebPage, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> DeleteContentAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> PublishContentAsync(Guid id, CancellationToken cancellationToken);

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
    }
}
