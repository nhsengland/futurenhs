using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco9ContentApi.Core.Models;

namespace UmbracoContentApi.Core.Converters
{
    public class JsonContentConverter : IConverter
    {
        public string EditorAlias => "Umbraco.TextArea";

        public object Convert(object value, Dictionary<string, object>? options = null)
        {
            return IsValidJson(value as string)
                ? JsonConvert.DeserializeObject<PageModel>(value as string)
                : value;
        }

        /// <summary>
        /// Determines whether [is valid json] [the specified string input].
        /// </summary>
        /// <param name="strInput">The string input.</param>
        /// <returns>
        ///   <c>true</c> if [is valid json] [the specified string input]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
            {
                return false;
            }

            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                var obj = JToken.Parse(strInput);

                if (obj is not null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
