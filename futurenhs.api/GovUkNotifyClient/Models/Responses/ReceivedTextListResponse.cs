using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Notify.Models.Responses
{
    public class ReceivedTextListResponse
    {
        [JsonPropertyName("received_text_messages")]
        public List<ReceivedTextResponse> receivedTexts;
        [JsonPropertyName("links")]
        public Link links;
    }
}
