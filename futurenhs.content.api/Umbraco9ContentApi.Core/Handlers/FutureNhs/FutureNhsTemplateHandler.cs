namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
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
        private List<string> errorList = new List<string>();
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
        public ApiResponse<ContentModelData> GetTemplate(Guid templateId, CancellationToken cancellationToken)
        {
            var template = _futureNhsContentService.GetPublishedContent(templateId, cancellationToken);
            var contentModelData = _futureNhsContentService.ResolvePublishedContent(template);
            return new ApiResponse<ContentModelData>().Success(contentModelData, "Template retrieved successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<IEnumerable<ContentModelData>> GetAllTemplates(CancellationToken cancellationToken)
        {
            ApiResponse<IEnumerable<ContentModelData>> response = new ApiResponse<IEnumerable<ContentModelData>>();
            var contentModels = new List<ContentModelData>();
            var templatesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Templates");
            var publishedTemplates = _futureNhsContentService.GetPublishedContent(templatesFolderGuid, cancellationToken).Children;

            if (publishedTemplates is not null && publishedTemplates.Any())
            {
                foreach (var templates in publishedTemplates)
                {
                    contentModels.Add(_futureNhsContentService.ResolvePublishedContent(templates));
                }
            }

            return new ApiResponse<IEnumerable<ContentModelData>>().Success(contentModels, "All templates retrieved successfully.");
        }
    }
}
