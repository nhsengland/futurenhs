using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Models.Content;
using Umbraco9ContentApi.Core.Resolvers.Interfaces;
using UmbracoContentApi.Core.Builder;
using ContentModelData = Umbraco9ContentApi.Core.Models.Content.ContentModelData;

namespace UmbracoContentApi.Core.Resolvers
{
    public class FutureNhsContentResolver : IFutureNhsContentResolver
    {
        private readonly ConverterCollection _converters;
        private readonly ILogger<FutureNhsContentResolver> _logger;
        private readonly IPublishedValueFallback _publishedValueFallback;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentService _contentService;

        public FutureNhsContentResolver(
            IVariationContextAccessor variationContextAccessor,
            ConverterCollection converters,
            ILogger<FutureNhsContentResolver> logger,
            IPublishedValueFallback publishedValueFallback, IContentTypeService contentTypeService, IContentService contentService)
        {
            _variationContextAccessor = variationContextAccessor;
            _converters = converters;
            _logger = logger;
            _publishedValueFallback = publishedValueFallback;
            _contentTypeService = contentTypeService;
            _contentService = contentService;
        }

        public ContentModelData ResolveContent(IPublishedElement content, string propertyGroupAlias, Dictionary<string, object>? options = null)
        {
            try
            {
                if (content is null)
                {
                    return new ContentModelData();
                }

                var contentType = _contentTypeService.Get(content.ContentType.Alias);
                var contentModel = new ContentModelData
                {
                    Item = new ContentModelItemData
                    {
                        Id = content.Key,
                        ContentType = content.ContentType.Alias,
                        Type = content.ContentType.ItemType.ToString()
                    }
                };

                var contentDictionary = new Dictionary<string, object>();

                if (content is IPublishedContent publishedContent)
                {
                    contentModel.Item.CreatedAt = publishedContent.CreateDate;
                    contentModel.Item.EditedAt = publishedContent.UpdateDate;
                    contentModel.Item.Locale = _variationContextAccessor.VariationContext.Culture;
                    contentModel.Item.Name = publishedContent.Name;
                    contentModel.Item.UrlSegment = publishedContent.UrlSegment;

                    if (options is not null &&
                        options.ContainsKey("addUrl") &&
                        bool.TryParse(options["addUrl"].ToString(), out var addUrl) &&
                        addUrl)
                    {
                        contentModel.Item.Url = publishedContent.Url(mode: UrlMode.Absolute);
                    }

                    // Add blocks to content if applicable (loops through all nested content)
                    List<ContentModelData> contentModelDataList = new();
                    if (publishedContent.Children is not null && publishedContent.Children.Any())
                    {
                        foreach (var child in publishedContent.Children.Where(x => x.ContentType.Alias is not GeneralWebPage.ModelTypeAlias))
                        {
                            contentModelDataList.Add(ResolveContent(child, "content"));
                        }
                        contentDictionary.Add("blocks", contentModelDataList);
                    }
                }

                var group = contentType.PropertyGroups.FirstOrDefault(x => x.Alias == propertyGroupAlias);
                var groupProperties = group?.PropertyTypes.Select(x => x.Alias);
                if (groupProperties is not null)
                {
                    foreach (var property in content.Properties.Where(x => groupProperties.Contains(x.Alias)))
                    {
                        var converter =
                            _converters.FirstOrDefault(x => x.EditorAlias.Equals(property.PropertyType.EditorAlias));
                        if (converter is not null)
                        {
                            var propertyValue = property.Value(_publishedValueFallback);

                            if (propertyValue is null)
                            {
                                contentDictionary.Add(property.Alias, null);
                                continue;
                            }

                            propertyValue = converter.Convert(propertyValue, options?.ToDictionary(x => x.Key, x => x.Value));

                            if (propertyValue is not null)
                            {
                                contentDictionary.Add(property.Alias, propertyValue);
                            }
                        }
                        else
                        {
                            contentDictionary.Add(
                                property.Alias,
                                $"No converter implemented for editor: {property.PropertyType.EditorAlias}");
                        }
                    }
                }

                contentModel.Content = contentDictionary;
                return contentModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occured when resolving content, see the inner exception for details");
                throw;
            }
        }

        public ContentModelData ResolveContent(IContent content, string propertyGroupAlias, Dictionary<string, object>? options = null)
        {
            try
            {
                if (content is null)
                {
                    return new ContentModelData();
                }

                var contentType = _contentTypeService.Get(content.ContentType.Alias);
                var contentModel = new ContentModelData
                {
                    Item = new ContentModelItemData
                    {
                        Id = content.Key,
                        ContentType = content.ContentType.Alias,
                        Name = content.Name,
                        Type = content.ContentType.Name
                    }
                };

                var contentDictionary = new Dictionary<string, object>();

                // Add blocks to content if applicable (loops through all nested content)
                if (_contentService.HasChildren(content.Id))
                {
                    List<ContentModelData> contentModelDataList = new();
                    long totalChildren;
                    var children = _contentService.GetPagedChildren(content.Id, 0, int.MaxValue, out totalChildren);
                    foreach (var child in children.Where(x => x.ContentType.Alias is not GeneralWebPage.ModelTypeAlias))
                    {
                        contentModelDataList.Add(ResolveContent(child, "content"));
                    }

                    contentDictionary.Add("blocks", contentModelDataList);
                }

                var group = contentType.PropertyGroups.FirstOrDefault(x => x.Alias == propertyGroupAlias);
                var groupProperties = group?.PropertyTypes.Select(x => x.Alias);

                foreach (var property in content.Properties.Where(x => groupProperties.Contains(x.Alias)))
                {
                    var converter =
                        _converters.FirstOrDefault(x => x.EditorAlias.Equals(property.PropertyType.PropertyEditorAlias));
                    if (converter is not null)
                    {
                        var propertyValue = property.GetValue();

                        if (propertyValue is null)
                        {
                            contentDictionary.Add(property.Alias, null);
                            continue;
                        }

                        propertyValue = converter.Convert(propertyValue, options?.ToDictionary(x => x.Key, x => x.Value));

                        contentDictionary.Add(property.Alias, propertyValue);
                    }
                    else
                    {
                        contentDictionary.Add(
                            property.Alias,
                            $"No converter implemented for editor: {property.PropertyType.PropertyEditorAlias}");
                    }
                }

                contentModel.Content = contentDictionary;
                return contentModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occured when resolving content, see the inner exception for details");
                throw;
            }
        }
    }
}
