using Umbraco.Cms.Core.Models.PublishedContent;
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
        private readonly Lazy<IFutureNhsContentResolver> _contentResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockPickerConverter"/> class.
        /// </summary>
        /// <param name="contentResolver">The content resolver.</param>
        public BlockPickerConverter(Lazy<IFutureNhsContentResolver> contentResolver)
        {
            _contentResolver = contentResolver;
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

            return ((IEnumerable<IPublishedElement>)value)
                                    .Select(x => _contentResolver.Value.ResolveContent(x))
                                    .ToList();
        }

    }
}
