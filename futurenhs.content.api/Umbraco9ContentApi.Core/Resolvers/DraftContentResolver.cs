﻿using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Resolvers.Interface;
using UmbracoContentApi.Core.Builder;
using ContentModel = Umbraco9ContentApi.Core.Models.ContentModel;

namespace UmbracoContentApi.Core.Resolvers
{
    public class DraftContentResolver : IDraftContentResolver
    {
        private readonly ConverterCollection _converters;
        private readonly ILogger<ContentResolver> _logger;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public DraftContentResolver(
            IVariationContextAccessor variationContextAccessor,
            ConverterCollection converters,
            ILogger<ContentResolver> logger)
        {
            _variationContextAccessor = variationContextAccessor;
            _converters = converters;
            _logger = logger;
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
                    Item = new Umbraco9ContentApi.Core.Models.ItemModel
                    {
                        Id = content.Key,
                        ContentType = content.ContentType.Alias,
                        Type = content.ContentType.Name
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