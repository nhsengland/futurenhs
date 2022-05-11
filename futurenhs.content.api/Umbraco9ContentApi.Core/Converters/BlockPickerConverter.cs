using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco9ContentApi.Core.Resolvers.Interfaces;
using UmbracoContentApi.Core.Converters;

namespace Umbraco9ContentApi.Core.Converters
{
    /// <summary>
    /// BlockPickerConverter generates content models for template blocks.
    /// </summary>
    /// <seealso cref="IConverter" />
    public sealed class BlockPickerConverter : IConverter
    {
        private readonly Lazy<IFutureNhsContentResolver> _futureNhsContentResolver;
        private readonly IContentService _contentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockPickerConverter"/> class.
        /// </summary>
        /// <param name="contentResolver">The content resolver.</param>
        public BlockPickerConverter(Lazy<IFutureNhsContentResolver> contentResolver, IContentService contentService)
        {
            _futureNhsContentResolver = contentResolver;
            _contentService = contentService;
        }

        /// <inheritdoc />
        public string EditorAlias => "Umbraco.MultiNodeTreePicker";

        /// <inheritdoc />
        public object Convert(object value, Dictionary<string, object>? options = null)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value), $"A value for {EditorAlias} is required.");
            }

            // IContent 
            if (value.GetType() == typeof(string))
            {
                var valueList = new List<IContent>();
                var udiList = value is not null ? value.ToString().Split(',').ToList() : new List<string>();

                if (!udiList.Any())
                    return null;

                foreach (var udi in udiList)
                {
                    var udiSubString = udi.Substring(udi.Length - 32);
                    var guid = Guid.ParseExact(udiSubString, "N");
                    var page = _contentService.GetById(guid);
                    valueList.Add(page);
                }

                return ((IEnumerable<IContent>)valueList)
                                   .Select(x => _futureNhsContentResolver.Value.ResolveContent(x))
                                   .ToList();
            }

            // IPubishedContent
            return ((IEnumerable<IPublishedElement>)value)
                                    .Select(x => _futureNhsContentResolver.Value.ResolveContent(x))
                                    .ToList();
        }

    }
}
