using System.Text.Json.Serialization;

namespace Notify.Models
{
    public class NotifyHTTPError
    {
        #pragma warning disable 169
        [JsonPropertyName("error")]
        private string error;

        [JsonPropertyName("message")]
        private string message;
    }
}
