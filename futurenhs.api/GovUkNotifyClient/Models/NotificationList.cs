using System.Collections.Generic;
using System.Text.Json.Serialization;
using Notify.Models.Responses;

namespace Notify.Models
{
    public class NotificationList
    {
        [JsonPropertyName("notifications")]
        public List<Notification> notifications;
        [JsonPropertyName("links")]
        public Link links;
    }
}
