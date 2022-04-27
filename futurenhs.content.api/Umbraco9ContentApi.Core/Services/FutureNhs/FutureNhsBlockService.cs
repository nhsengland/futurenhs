using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;
using UmbracoContentApi.Core.Builder;

namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    public class FutureNhsBlockService : IFutureNhsBlockService
    {
        private readonly ConverterCollection _converters;
        private readonly IFutureNhsContentService _futureNhsContentService;

        public FutureNhsBlockService(ConverterCollection converters, IFutureNhsContentService futureNhsContentService)
        {
            _converters = converters;
            _futureNhsContentService = futureNhsContentService;
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
    }
}
