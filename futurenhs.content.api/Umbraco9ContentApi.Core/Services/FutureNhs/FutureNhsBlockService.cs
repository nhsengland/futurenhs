﻿using Umbraco.Cms.Core.Services;
using Umbraco9ContentApi.Core.Models.Requests;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;
using UmbracoContentApi.Core.Builder;

namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    public class FutureNhsBlockService : IFutureNhsBlockService
    {
        private readonly ConverterCollection _converters;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentService _contentService;

        public FutureNhsBlockService(ConverterCollection converters, IFutureNhsContentService futureNhsContentService, IContentTypeService contentTypeService, IContentService contentService)
        {
            _converters = converters;
            _futureNhsContentService = futureNhsContentService;
            _contentTypeService = contentTypeService;
            _contentService = contentService;
        }

        public async Task<IEnumerable<string?>> GetBlockPlaceholderValuesAsync(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            var block = await _futureNhsContentService.GetPublishedContentAsync(blockId, cancellationToken);

            var contentType = _contentTypeService.Get(block.ContentType.Alias);
            var group = contentType.PropertyGroups.FirstOrDefault(x => x.Alias == propertyGroupAlias);
            var groupProperties = group?.PropertyTypes.Select(x => x.Alias);

            return (from property in block.Properties.Where(x => groupProperties != null && groupProperties.Contains(x.Alias))
                    let converter = _converters.FirstOrDefault(x => x.EditorAlias.Equals(property.PropertyType.EditorAlias))
                    where converter != null
                    select property.GetValue().ToString()).ToList();
        }

        public async Task<Umbraco.Cms.Core.Models.IContent> CreateBlockAsync(CreateBlockRequest createRequest, Guid blocksDataSourceFolderGuid, CancellationToken cancellationToken)
        {
            var blockDataSourceFolder = _contentService.GetById(blocksDataSourceFolderGuid);
            var result = _contentService.CreateAndSave(createRequest.Name, blockDataSourceFolder.Id, createRequest.ContentType);
            _contentService.SaveAndPublish(result); // will publishing as we create a block affect page publishing?
            return result;
        }
    }
}
