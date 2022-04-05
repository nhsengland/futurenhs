using Newtonsoft.Json;
using Umbraco9ContentApi.Core.Models;

namespace UmbracoContentApi.Core.Converters
{
    public class JsonContentConverter : IConverter
    {
        public string EditorAlias => "Umbraco.TextArea";

        public object Convert(object value, Dictionary<string, object>? options = null)
        {
            return JsonConvert.DeserializeObject<PageContentModelData>(value as string);
        }
    }
}