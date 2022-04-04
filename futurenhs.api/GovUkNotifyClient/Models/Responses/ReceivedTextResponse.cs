using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Notify.Models.Responses
{
    public class ReceivedTextResponse
    {
        public string id;
        [JsonPropertyName("created_at")]
        public string createdAt;
        [JsonPropertyName("notify_number")]
        public string notifyNumber;
        [JsonPropertyName("user_number")]
        public string userNumber;
        [JsonPropertyName("service_id")]
        public string serviceId;
        public string content;

        public override bool Equals(object receivedText)
        {
            if (!(receivedText is ReceivedTextResponse text))
            {
                return false;
            }

            return
                id == text.id &&
                createdAt == text.createdAt &&
                notifyNumber == text.notifyNumber &&
                userNumber == text.userNumber &&
                serviceId == text.serviceId &&
                content == text.content;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
