using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco9ContentApi.Core.Models.Blocks;
using Umbraco9ContentApi.Core.Resolvers.Interfaces;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;
using UmbracoContentApi.Core.Builder;

namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    public class FutureNhsBlockService : IFutureNhsBlockService
    {
        private readonly ConverterCollection _converters;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsBlockResolver _futureNhsBlockResolver;

        public FutureNhsBlockService(ConverterCollection converters, IFutureNhsContentService futureNhsContentService, IFutureNhsBlockResolver futureNhsBlockResolver)
        {
            _converters = converters;
            _futureNhsContentService = futureNhsContentService;
            _futureNhsBlockResolver = futureNhsBlockResolver;
        }

        public async Task<IEnumerable<string>> GetBlockPlaceholderValuesAsync(Guid blockId)
        {
            List<string> placeholderValues = new List<string>();
            var block = _futureNhsContentService.GetPublishedAsync(blockId);

            foreach (var property in block.Result.Properties)
            {
                var converter =
                        _converters.FirstOrDefault(x => x.EditorAlias.Equals(property.PropertyType.EditorAlias));
                if (converter != null)
                {
                    placeholderValues.Add(property.GetValue().ToString());
                }
            }

            return placeholderValues;
        }

        public async Task<BlockModel> ResolvePublishedBlockAsync(IPublishedContent block)
        {
            return _futureNhsBlockResolver.ResolveContent(block);
        }
    }
}
