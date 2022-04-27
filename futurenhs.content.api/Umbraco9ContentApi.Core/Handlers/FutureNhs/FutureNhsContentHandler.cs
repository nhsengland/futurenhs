namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Services.FutureNhs.Interface;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Response;

    /// <summary>
    /// The handler that handles content methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsContentHandler" />
    public sealed class FutureNhsContentHandler : IFutureNhsContentHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsValidationService _futureNhsValidationService;
        private List<string>? errorList = null;

        public FutureNhsContentHandler(IConfiguration config, IFutureNhsContentService futureNhsContentService, IFutureNhsValidationService futureNhsValidationService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _futureNhsContentService = futureNhsContentService ?? throw new ArgumentNullException(nameof(futureNhsContentService));
            _futureNhsValidationService = futureNhsValidationService ?? throw new ArgumentNullException(nameof(futureNhsValidationService));
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> CreateContentAsync(string pageName, string? parentId = null, bool publish = false)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            Guid pageParentGuid;
            var pageFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");

            // If a parent page id is supplied and is a valid guid, set that page as the page parent. Else use the pages folder.
            Guid parent = parentId is not null && Guid.TryParse(parentId, out pageParentGuid)
                ? pageParentGuid
                : pageFolderGuid;

            var pageDocumentTypeAlias = GeneralWebPage.ModelTypeAlias;

            var result = _futureNhsContentService.CreateAsync(parent, pageName, pageDocumentTypeAlias).Result;

            if (result is null)
            {
                errorList.Add("Content creation failed.");
                return response.Failure(errorList, "Failed.");
            }

            return response.Success(result.Key.ToString(), "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> UpdateContentAsync(Guid id, string title, string description, PageContentModel pageContent)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(description) && pageContent is not null)
            {
                errorList.Add("No data provided.");
                return response.Failure(errorList, "Failed.");
            }

            _futureNhsValidationService.ValidatePageContentModel(pageContent);

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

            if (pageContent is null)
            {
                pageTemplateContent.SetValue("pageContent", JsonConvert.SerializeObject(pageContent));
            }

            var result = await _futureNhsContentService.SaveAndPublishAsync(pageTemplateContent);

            if (result)
            {
                return response.Success(id.ToString(), "Success.");
            }

            errorList.Add("Error occured.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> PublishContentAsync(Guid contentId)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var result = await _futureNhsContentService.PublishAsync(contentId);

            if (result)
            {
                return response.Success(result.ToString(), "Success.");
            }

            errorList.Add("Publish failed.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<ContentModel>> GetContentAsync(Guid id)
        {
            ApiResponse<ContentModel> response = new ApiResponse<ContentModel>();
            var content = await _futureNhsContentService.GetPublishedAsync(id);
            var result = await _futureNhsContentService.ResolveAsync(content);

            if (result is not null)
            {
                return response.Success(result, "Success.");
            }

            errorList.Add("Couldn't retrieve content.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> DeleteContentAsync(Guid id)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var result = await _futureNhsContentService.DeleteAsync(id);

            if (result)
            {
                return response.Success(id.ToString(), "Success.");
            }

            errorList.Add("Couldn't delete content.");
            return response.Failure(errorList, "Failed.");
        }



        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<ContentModel>>> GetAllContentAsync()
        {
            ApiResponse<IEnumerable<ContentModel>> response = new ApiResponse<IEnumerable<ContentModel>>();
            var contentModels = new List<ContentModel>();
            var pagesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");
            var publishedContent = await _futureNhsContentService.GetPublishedChildrenAsync(pagesFolderGuid);

            if (publishedContent is not null && publishedContent.Any())
            {
                foreach (var content in publishedContent)
                {
                    contentModels.Add(await _futureNhsContentService.ResolveAsync(content));
                }
            }

            return response.Success(contentModels, "Success.");
        }
    }
}
