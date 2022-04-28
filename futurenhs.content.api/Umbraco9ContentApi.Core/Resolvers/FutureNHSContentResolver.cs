using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Models.Content;
using Umbraco9ContentApi.Core.Resolvers.Interfaces;
using UmbracoContentApi.Core.Builder;
using ContentModel = Umbraco9ContentApi.Core.Models.Content.ContentModel;

namespace UmbracoContentApi.Core.Resolvers
{
    public class FutureNhsContentResolver : IFutureNhsContentResolver
    {
        private readonly ConverterCollection _converters;
        private readonly ILogger<FutureNhsContentResolver> _logger;
        private readonly IPublishedValueFallback _publishedValueFallback;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IContentTypeService _contentTypeService;

        public FutureNhsContentResolver(
            IVariationContextAccessor variationContextAccessor,
            ConverterCollection converters,
            ILogger<FutureNhsContentResolver> logger,
            IPublishedValueFallback publishedValueFallback, IContentTypeService contentTypeService)
        {
            _variationContextAccessor = variationContextAccessor;
            _converters = converters;
            _logger = logger;
            _publishedValueFallback = publishedValueFallback;
            _contentTypeService = contentTypeService;
        }

        public ContentModel ResolveContent(IPublishedElement content, string propertyGroupAlias, Dictionary<string, object>? options = null)
        {
            try
            {
                if (content == null)
                {
                    throw new ArgumentNullException(nameof(content));
                }

                var contentType = _contentTypeService.Get(content.ContentType.Alias);
                var group = contentType.PropertyGroups.FirstOrDefault(x => x.Alias == propertyGroupAlias);
                var groupProperties = group?.PropertyTypes.Select(x => x.Alias);

                if (groupProperties == null)
                {
                    throw new ArgumentNullException(nameof(groupProperties));

                }

                var contentModel = new ContentModel
                {
                    Item = new ContentModelItem
                    {
                        Id = content.Key,
                        ContentType = content.ContentType.Alias,
                        Type = content.ContentType.ItemType.ToString()
                    }
                };

                var dict = new Dictionary<string, object>();

                if (content is IPublishedContent publishedContent)
                {
                    contentModel.Item.CreatedAt = publishedContent.CreateDate;
                    contentModel.Item.EditedAt = publishedContent.UpdateDate;
                    contentModel.Item.Locale = _variationContextAccessor.VariationContext.Culture;
                    contentModel.Item.Name = publishedContent.Name;
                    contentModel.Item.UrlSegment = publishedContent.UrlSegment;

                    if (options != null &&
                        options.ContainsKey("addUrl") &&
                        bool.TryParse(options["addUrl"].ToString(), out var addUrl) &&
                        addUrl)
                    {
                        contentModel.Item.Url = publishedContent.Url(mode: UrlMode.Absolute);
                    }
                }

                foreach (var property in content.Properties.Where(x => groupProperties.Contains(x.Alias)))
                {
                    var converter =
                        _converters.FirstOrDefault(x => x.EditorAlias.Equals(property.PropertyType.EditorAlias));
                    if (converter != null)
                    {
                        var prop = property.Value(_publishedValueFallback);

                        if (prop == null)
                        {
                            continue;
                        }

                        prop = converter.Convert(prop, options?.ToDictionary(x => x.Key, x => x.Value));

                        if (prop != null)
                        {
                            dict.Add(property.Alias, prop);
                        }
                    }
                    else
                    {
                        dict.Add(
                            property.Alias,
                            $"No converter implemented for editor: {property.PropertyType.EditorAlias}");
                    }
                }

                contentModel.Content = dict;

                return contentModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exceptional exception happened, see the inner exception for details");
                throw;
            }
        }

        public ContentModel ResolveContent(IContent content, Dictionary<string, object>? options = null)
        {
            try
            {
                if (content == null)
                {
                    throw new ArgumentNullException(nameof(content));
                }

                var contentModel = new ContentModel
                {
                    Item = new ContentModelItem
                    {
                        Id = content.Key,
                        ContentType = content.ContentType.Alias,
                        Name = content.Name,
                        Type = content.ContentType.Name
                    }
                };

                var dict = new Dictionary<string, object>();

                foreach (var property in content.Properties)
                {
                    var converter =
                        _converters.FirstOrDefault(x => x.EditorAlias.Equals(property.PropertyType.PropertyEditorAlias));
                    if (converter != null)
                    {
                        var prop = property.GetValue();

                        if (prop == null)
                        {
                            continue;
                        }

                        prop = converter.Convert(prop, options?.ToDictionary(x => x.Key, x => x.Value));

                        if (prop != null)
                        {
                            dict.Add(property.Alias, prop);
                        }
                    }
                    else
                    {
                        dict.Add(
                            property.Alias,
                            $"No converter implemented for editor: {property.PropertyType.PropertyEditorAlias}");
                    }
                }

                contentModel.Content = dict;
                return contentModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exceptional exception happened, see the inner exception for details");
                throw;
            }
        }
    }
}
