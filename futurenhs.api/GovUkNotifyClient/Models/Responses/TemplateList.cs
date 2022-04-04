
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Notify.Models.Responses
{
    public class TemplateList
    {
        [JsonPropertyName("templates")]
        public List<TemplateResponse> templates;
    }
}
