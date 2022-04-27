using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Models.Blocks;
using Umbraco9ContentApi.Core.Resolvers.Interfaces;
using UmbracoContentApi.Core.Builder;

namespace UmbracoContentApi.Core.Resolvers
{
    public class FutureNhsBlockResolver : IFutureNhsBlockResolver
    {
        private readonly ConverterCollection _converters;
        private readonly ILogger<FutureNhsContentResolver> _logger;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public FutureNhsBlockResolver(
            IVariationContextAccessor variationContextAccessor,
            ConverterCollection converters,
            ILogger<FutureNhsContentResolver> logger)
        {
            _variationContextAccessor = variationContextAccessor;
            _converters = converters;
            _logger = logger;
        }

        public BlockModel ResolveContent(IPublishedElement content, Dictionary<string, object>? options = null)
        {
            try
            {
                if (content == null)
                {
                    throw new ArgumentNullException(nameof(content));
                }

                var blockModel = new BlockModel
                {
                    Item = new ItemModel
                    {
                        Id = content.Key,
                        ContentType = content.ContentType.Alias,
                        Type = content.ContentType.ItemType.ToString()
                    }
                };

                var fields = new List<string>();


                if (content is IPublishedContent publishedContent)
                {
                    blockModel.Item.CreatedAt = publishedContent.CreateDate;
                    blockModel.Item.EditedAt = publishedContent.UpdateDate;
                    blockModel.Item.Locale = _variationContextAccessor.VariationContext.Culture;
                    blockModel.Item.Name = publishedContent.Name;
                    blockModel.Item.UrlSegment = publishedContent.UrlSegment;

                    if (options != null &&
                        options.ContainsKey("addUrl") &&
                        bool.TryParse(options["addUrl"].ToString(), out var addUrl) &&
                        addUrl)
                    {
                        blockModel.Item.Url = publishedContent.Url(mode: UrlMode.Absolute);
                    }
                }

                foreach (var property in content.Properties)
                {

                    fields.Add(property.Alias);
                }

                blockModel.Fields = fields;
                return blockModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exceptional exception happened, see the inner exception for details");
                throw;
            }
        }
    }
}