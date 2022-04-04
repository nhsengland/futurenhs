using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Notify.Models.Responses
{
    public class NotifyHTTPErrorResponse
    {
        #pragma warning disable 169, 649
        [JsonPropertyName("status_code")]
        private string statusCode;

        [JsonPropertyName("errors")]
        private List<NotifyHTTPError> errors;

        public string getStatusCode()
        {
            return statusCode;
        }

        public string getErrorsAsJson()
        {
            return JsonSerializer.Serialize(errors, new JsonSerializerOptions());
        }
    }
}
