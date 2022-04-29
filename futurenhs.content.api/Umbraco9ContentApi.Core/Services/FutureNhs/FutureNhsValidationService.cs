namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using System;
    using Umbraco.Cms.Core.Services;
    using Umbraco9ContentApi.Core.Models;
    using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;

    public class FutureNhsValidationService : IFutureNhsValidationService
    {
        private readonly IContentTypeService _contentTypeService;

        public FutureNhsValidationService(IContentTypeService contentTypeService)
        {
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
        }

        public void ValidatePageContentModel(PageContentModel pageContentModel)
        {
            foreach (var block in pageContentModel.Blocks)
            {
                var contentType = _contentTypeService.Get(block.Item?.ContentType);

                if (contentType is null)
                {
                    throw new ArgumentOutOfRangeException($"{block.Item?.ContentType} isn't a valid content type.");
                }

                var expectedValues = contentType.PropertyTypes.Select(x => x.Alias).ToList();
                var blockValues = block.Content?.Select(x => x.Key.ToString()).ToList();

                if (blockValues is not null)
                {
                    foreach (var value in blockValues)
                    {
                        if (!expectedValues.Contains(value))
                            throw new ArgumentOutOfRangeException($"Block fields do not match the expected fields for contentType {contentType.Name}.");
                    }
                }
            }
        }
    }
}
