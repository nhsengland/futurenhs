namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;
    using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;


    /// <summary>
    /// The handler that handles template methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsTemplateHandler" />
    public sealed class FutureNhsTemplateHandler : IFutureNhsTemplateHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private List<string>? errorList = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureNhsTemplateHandler"/> class.
        /// </summary>
        /// <param name="futureNhsContentService">The future NHS content service.</param>
        /// <param name="config">The configuration.</param>
        public FutureNhsTemplateHandler(IFutureNhsContentService futureNhsContentService, IConfiguration config)
        {
            _futureNhsContentService = futureNhsContentService;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<ApiResponse<ContentModel>> GetTemplateAsync(Guid id)
        {
            ApiResponse<ContentModel> response = new ApiResponse<ContentModel>();
            var template = await _futureNhsContentService.GetPublishedAsync(id);
            var result = await _futureNhsContentService.ResolveAsync(template);

            if (result is null)
            {
                errorList.Add("Couldn't retrieve template.");
                return response.Failure(errorList, "Failed.");
            }

            return response.Success(result, "Success.");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<IEnumerable<ContentModel>>> GetAllTemplatesAsync()
        {
            ApiResponse<IEnumerable<ContentModel>> response = new ApiResponse<IEnumerable<ContentModel>>();
            var contentModels = new List<ContentModel>();
            var templatesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Templates");
            var publishedTemplates = await _futureNhsContentService.GetPublishedChildrenAsync(templatesFolderGuid);

            if (publishedTemplates is not null && publishedTemplates.Any())
            {
                foreach (var templates in publishedTemplates)
                {
                    contentModels.Add(await _futureNhsContentService.ResolveAsync(templates));
                }
            }

            return response.Success(contentModels, "Success.");
        }
    }
}