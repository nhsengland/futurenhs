namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using Microsoft.Extensions.Logging;
    using System;
    using Umbraco.Cms.Core.Services;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;

    public class FutureNhsValidationService : IFutureNhsValidationService
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly ILogger<FutureNhsValidationService> _logger;

        public FutureNhsValidationService(IContentTypeService contentTypeService, ILogger<FutureNhsValidationService> logger)
        {
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
            _logger = logger;
        }

        /// <inheritdoc />
        public void ValidateContentModelData(ContentModelData contentModel)
        {
            if (contentModel.Item is not null && contentModel.Item.ContentType is not null)
            {
                var contentType = _contentTypeService.Get(contentModel.Item.ContentType);

                if (contentType is null)
                {
                    _logger.LogError("No content type for {ContentType} found.", contentModel.Item.ContentType);
                    throw new KeyNotFoundException($"No content type for {contentModel.Item.ContentType} found.");
                }

                var expectedValues = contentType.PropertyTypes.Select(x => x.Alias).ToList();

                if (contentModel.Content is not null)
                {
                    var fields = contentModel.Content
                                        .Select(x => x.Key.ToString())
                                        .Where(x => x != "blocks")
                                        .ToList(); // ignore 'blocks' as they're not contentType fields but are used to identify contentModel children in Umbraco.

                    if (fields is not null)
                    {
                        foreach (var field in fields)
                        {
                            if (!expectedValues.Contains(field))
                            {
                                _logger.LogError("No content field value for {FieldName} found.", field);
                                throw new ArgumentOutOfRangeException($"No content field value for {field} found.");
                            }
                        }
                    }
                }
            }
        }
    }
}
