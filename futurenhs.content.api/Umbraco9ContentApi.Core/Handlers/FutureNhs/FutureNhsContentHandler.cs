namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Services.FutureNhs.Interface;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;

    /// <summary>
    /// The handler that handles content methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsContentHandler" />
    public class FutureNhsContentHandler : IFutureNhsContentHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureNhsContentHandler" /> class.
        /// </summary>
        /// <param name="futureNhsContentService">The future NHS content service.</param>
        /// <param name="config">The configuration.</param>
        public FutureNhsContentHandler(IFutureNhsContentService futureNhsContentService, IConfiguration config)
        {
            _futureNhsContentService = futureNhsContentService;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<IContent?> CreateContentAsync(string pageName, string? pageParentId = null, bool publish = false)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                return null;
            }

            Guid pageParentGuid;

            var pageFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");

            // If a parent page id is supplied and is a valid guid, set that page as the page parent. Else use the pages folder.
            Guid parent = pageParentId is not null && Guid.TryParse(pageParentId, out pageParentGuid)
                ? pageParentGuid
                : pageFolderGuid;

            var pageDocumentTypeAlias = GeneralWebPage.ModelTypeAlias;

            // if publish is true, also publish the created page.
            if (publish)
            {
                var createResult = await _futureNhsContentService.CreateAsync(parent, pageName, pageDocumentTypeAlias);

                if (createResult is not null)
                {
                    await PublishContentAsync(createResult.Key);
                }

                return createResult;
            }

            return await _futureNhsContentService.CreateAsync(parent, pageName, pageDocumentTypeAlias);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateContentAsync(Guid id, string title, string description, string pageContent)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(description) && string.IsNullOrWhiteSpace(pageContent))
            {
                return false;
            }

            var pageTemplateContent = await _futureNhsContentService.GetAsync(id);

            if (!string.IsNullOrWhiteSpace(title))
            {
                pageTemplateContent.Name = title;
                pageTemplateContent.SetValue("title", title);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                pageTemplateContent.SetValue("description", description);
            }

            if (!string.IsNullOrWhiteSpace(pageContent))
            {
                pageTemplateContent.SetValue("pageContent", pageContent);
            }

            var pagePublishedContent = await _futureNhsContentService.GetPublishedAsync(id);

            if (pagePublishedContent is not null && pagePublishedContent.IsPublished())
            {
                var result = await _futureNhsContentService.SaveAndPublishAsync(pageTemplateContent);
                return result;
            }
            else
            {
                var result = await _futureNhsContentService.SaveAsync(pageTemplateContent);
                return result;
            }
        }

        /// <inheritdoc />
        public async Task<bool> PublishContentAsync(Guid id)
        {
            return await _futureNhsContentService.PublishAsync(id);
        }

        /// <inheritdoc />
        public async Task<ContentModel> GetContentAsync(Guid id)
        {
            var content = await _futureNhsContentService.GetPublishedAsync(id);
            return await _futureNhsContentService.ResolveAsync(content);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteContentAsync(Guid id)
        {
            return await _futureNhsContentService.DeleteAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ContentModel>> GetAllContentAsync()
        {
            var contentModels = new List<ContentModel>();
            var pagesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");
            var pages = await _futureNhsContentService.GetPublishedChildrenAsync(pagesFolderGuid);

            foreach (var page in pages)
            {
                contentModels.Add(await _futureNhsContentService.ResolveAsync(page));
            }

            return contentModels;
        }
    }
}
