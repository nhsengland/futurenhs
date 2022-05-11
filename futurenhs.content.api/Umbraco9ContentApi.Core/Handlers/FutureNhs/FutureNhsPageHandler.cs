namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Services.FutureNhs.Interface;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Web.Common.PublishedModels;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;
    using static Umbraco.Cms.Core.Constants;

    /// <summary>
    /// The handler that handles content methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsContentHandler" />
    public sealed class FutureNhsPageHandler : IFutureNhsPageHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsValidationService _futureNhsValidationService;
        private readonly IContentTypeService _contentTypeService;
        private List<string> errorList = new List<string>();

        public FutureNhsPageHandler(IConfiguration config, IFutureNhsContentService futureNhsContentService, IFutureNhsValidationService futureNhsValidationService, IContentTypeService contentTypervice)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _futureNhsContentService = futureNhsContentService ?? throw new ArgumentNullException(nameof(futureNhsContentService));
            _futureNhsValidationService = futureNhsValidationService ?? throw new ArgumentNullException(nameof(futureNhsValidationService));
            _contentTypeService = contentTypervice;
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> CreatePageAsync(string pageName, string pageParentId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            Guid pageParentGuid;
            var pageFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");

            // If a parent page id is supplied and is a valid guid, set that page as the page parent. Else use the pages folder.
            Guid parentId = pageParentId is not null && Guid.TryParse(pageParentId, out pageParentGuid)
                ? pageParentGuid
                : pageFolderGuid;

            var result = _futureNhsContentService.CreateContentAsync(pageName, parentId, GeneralWebPage.ModelTypeAlias, cancellationToken).Result;

            if (result is null)
            {
                errorList.Add("Content creation failed.");
                return response.Failure(errorList, "Failed.");
            }

            return response.Success(result.Key.ToString(), "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> UpdatePageAsync(Guid pageId, PageModel pageModel, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            bool result;

            if (pageModel is null || !pageModel.Blocks.Any())
            {
                errorList.Add("No data provided.");
                return response.Failure(errorList, "Failed.");
            }

            _futureNhsValidationService.ValidatePageContentModel(pageModel);

            var pageToUpdate = await _futureNhsContentService.GetDraftContentAsync(pageId, cancellationToken);

            List<string> blockUdisList = new List<string>();

            foreach (var block in pageModel.Blocks)
            {
                // Convert block item guid to a udi (required by umbraco)
                string blockUdi = Udi.Create(UdiEntityType.Document, block.Item.Id).ToString();
                blockUdisList.Add(blockUdi);

                _futureNhsValidationService.ValidateContentModel(block);

                var blockToUpdate = await _futureNhsContentService.GetDraftContentAsync(block.Item.Id, cancellationToken);

                // Get the block blockType
                var blockType = _contentTypeService.Get(block.Item?.ContentType);

                // Loop through the blockType properties
                foreach (var property in blockType.PropertyTypes)
                {
                    var updateValue = block.Content.Where(p => p.Key == property.Alias).Select(x => x.Value).FirstOrDefault();

                    // If the property has child/nested block we set the value appropriately
                    if (property.PropertyEditorAlias is "Umbraco.MultiNodeTreePicker")
                    {
                        var blockModelsObject = JsonConvert.SerializeObject(updateValue);
                        var blockModels = JsonConvert.DeserializeObject<List<ContentModel>>(blockModelsObject);

                        if (blockModels is not null)
                        {
                            foreach (var model in blockModels)
                            {
                                // Convert block item guid to a udi (required by umbraco)
                                blockUdi = Udi.Create(UdiEntityType.Document, model.Item.Id).ToString();
                                blockUdisList.Add(blockUdi);
                            }

                            blockToUpdate.Properties[$"{property.Alias}"].SetValue(string.Join(",", blockUdisList));
                        }
                    }
                    else
                    {
                        blockToUpdate.Properties[$"{property.Alias}"].SetValue(updateValue);
                    }
                }

                result = await _futureNhsContentService.SaveContentAsync(blockToUpdate, cancellationToken);

                if (!result)
                {
                    errorList.Add("Could not update blocks.");
                    return response.Failure(errorList, "Failed.");
                }
            }

            pageToUpdate.Properties[$"blocks"].SetValue(string.Join(",", blockUdisList));

            result = await _futureNhsContentService.SaveContentAsync(pageToUpdate, cancellationToken);

            if (result)
            {
                return response.Success(pageId.ToString(), "Success.");
            }

            errorList.Add("Error occured.");
            return response.Failure(errorList, "Failed.");
        }


        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<ContentModel>>> GetAllPagesAsync(CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<ContentModel>> response = new ApiResponse<IEnumerable<ContentModel>>();
            var contentModels = new List<ContentModel>();
            var pagesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Groups");
            var publishedContent = await _futureNhsContentService.GetPublishedContentChildrenAsync(pagesFolderGuid, cancellationToken);

            if (publishedContent is not null && publishedContent.Any())
            {
                foreach (var content in publishedContent)
                {
                    contentModels.Add(await _futureNhsContentService.ResolvePublishedContentAsync(content, "content", cancellationToken));
                }
            }

            return response.Success(contentModels, "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            bool result;

            result = await _futureNhsContentService.UpdateUserEditingContentAsync(userId, pageId, cancellationToken);

            if (result)
            {
                return response.Success(result.ToString(), "Success.");
            }

            errorList.Add("Could not update edit status.");
            return response.Failure(errorList, "Failed.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> CheckPageEditStatusAsync(Guid pageId, CancellationToken cancellationToken)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var draftPage = await _futureNhsContentService.GetDraftContentAsync(pageId, cancellationToken);

            if (!draftPage.Edited)
            {
                return response.Success(string.Empty, "Success.");
            }

            var userId = draftPage.Properties.Where(x => x.Alias == "userEditing")
                ?.Select(p => p.GetValue())
                ?.FirstOrDefault()
                ?.ToString();

            return string.IsNullOrEmpty(userId)
                ? response.Failure(null, "Failed to retrieve userId")
                : response.Success(userId, "Success.");
        }
    }
}
