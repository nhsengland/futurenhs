namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Services.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Dto;
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
        public async Task<ApiResponse<string>> CreateContentAsync(string pageName, string pageParentId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            Guid pageParentGuid;
            var pageFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");

            // If a parent page id is supplied and is a valid guid, set that page as the page parent. Else use the pages folder.
            Guid parentId = pageParentId is not null && Guid.TryParse(pageParentId, out pageParentGuid)
                ? pageParentGuid
                : pageFolderGuid;

            var generalWebPageDto = new GeneralWebPageDto()
            {
                PageName = pageName,
                PageParentId = parentId,
            };

            var result = _futureNhsContentService.CreateContentAsync(generalWebPageDto, cancellationToken).Result;

            if (result is null)
            {
                errorList.Add("Content creation failed.");
                return response.Failure(errorList, "Failed.");
            }

            return response.Success(result.Key.ToString(), "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> UpdateContentAsync(Guid id, string title, string description, PageContentModel pageContent, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(description) && pageContent is not null)
            {
                errorList.Add("No data provided.");
                return response.Failure(errorList, "Failed.");
            }

            _futureNhsValidationService.ValidatePageContentModel(pageContent);

            var pageTemplateContent = await _futureNhsContentService.GetDraftContentAsync(id, cancellationToken);

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

            var result = await _futureNhsContentService.SaveContentAsync(pageTemplateContent, cancellationToken);

            if (result)
            {
                return response.Success(id.ToString(), "Success.");
            }

            errorList.Add("Error occured.");
            return response.Failure(errorList, "Failed.");
        }

        public async Task<ApiResponse<string>> UpdateContentAsync(Guid id, PageContentModel pageContent, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            _futureNhsValidationService.ValidatePageContentModel(pageContent);

            var pageTemplateContent = await _futureNhsContentService.GetDraftContentAsync(id, cancellationToken);

            if (pageContent is null)
            {
                pageTemplateContent.SetValue("pageContent", JsonConvert.SerializeObject(pageContent));
            }

            var result = await _futureNhsContentService.SaveContentAsync(pageTemplateContent, cancellationToken);

            if (result)
            {
                return response.Success(id.ToString(), "Success.");
            }

            errorList.Add("Error occured.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> PublishContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var result = await _futureNhsContentService.PublishContentAsync(contentId, cancellationToken);

            if (result)
            {
                return response.Success(result.ToString(), "Success.");
            }

            errorList.Add("Publish failed.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<ContentModel>> GetContentPublishedAsync(Guid id, CancellationToken cancellationToken)
        {
            ApiResponse<ContentModel> response = new ApiResponse<ContentModel>();
            var content = await _futureNhsContentService.GetPublishedContentAsync(id, cancellationToken);
            var result = await _futureNhsContentService.ResolvePublishedContentAsync(content, cancellationToken);

            if (result is not null)
            {
                return response.Success(result, "Success.");
            }

            errorList.Add("Couldn't retrieve content.");
            return response.Failure(errorList, "Failed.");
        }

        public async Task<ApiResponse<ContentModel>> GetContentDraftAsync(Guid id, CancellationToken cancellationToken)
        {
            ApiResponse<ContentModel> response = new ApiResponse<ContentModel>();
            var content = await _futureNhsContentService.GetDraftContentAsync(id, cancellationToken);
            var result = await _futureNhsContentService.ResolveDraftContentAsync(content, cancellationToken);

            if (result is not null)
            {
                return response.Success(result, "Success.");
            }

            errorList.Add("Couldn't retrieve content.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> DeleteContentAsync(Guid id, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var result = await _futureNhsContentService.DeleteContentAsync(id, cancellationToken);

            if (result)
            {
                return response.Success(id.ToString(), "Success.");
            }

            errorList.Add("Couldn't delete content.");
            return response.Failure(errorList, "Failed.");
        }



        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<ContentModel>>> GetAllContentAsync(CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<ContentModel>> response = new ApiResponse<IEnumerable<ContentModel>>();
            var contentModels = new List<ContentModel>();
            var pagesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");
            var publishedContent = await _futureNhsContentService.GetPublishedContentChildrenAsync(pagesFolderGuid, cancellationToken);

            if (publishedContent is not null && publishedContent.Any())
            {
                foreach (var content in publishedContent)
                {
                    contentModels.Add(await _futureNhsContentService.ResolvePublishedContentAsync(content, cancellationToken));
                }
            }

            return response.Success(contentModels, "Success.");
        }
    }
}
