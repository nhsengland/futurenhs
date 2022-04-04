using System.Text.Json.Serialization;

namespace Notify.Models
{
    public class Notification
    {
        public string id;
        [JsonPropertyName("completed_at")]
        public string completedAt;
        [JsonPropertyName("created_at")]
        public string createdAt;
        [JsonPropertyName("email_address")]
        public string emailAddress;
        public string body;
        public string subject;
        [JsonPropertyName("line_1")]
        public string line1;
        [JsonPropertyName("line_2")]
        public string line2;
        [JsonPropertyName("line_3")]
        public string line3;
        [JsonPropertyName("line_4")]
        public string line4;
        [JsonPropertyName("line_5")]
        public string line5;
        [JsonPropertyName("line_6")]
        public string line6;
        [JsonPropertyName("phone_number")]
        public string phoneNumber;
        public string postcode;
        public string postage;
        public string reference;
        [JsonPropertyName("sent_at")]
        public string sentAt;
        public string status;
        public Template template;
        public string type;
        [JsonPropertyName("created_by_name")]
        public string createdByName;


        public override bool Equals(object notification)
        {
            if (!(notification is Notification note))
            {
                return false;
            }

            return
                id == note.id &&
                completedAt == note.completedAt &&
                createdAt == note.createdAt &&
                emailAddress == note.emailAddress &&
                line1 == note.line1 &&
                line2 == note.line2 &&
                line3 == note.line3 &&
                line4 == note.line4 &&
                line5 == note.line5 &&
                line6 == note.line6 &&
                phoneNumber == note.phoneNumber &&
                postcode == note.postcode &&
                postage == note.postage &&
                reference == note.reference &&
                sentAt == note.sentAt &&
                status == note.status &&
                template.id == note.template.id &&
                template.uri == note.template.uri &&
                template.version == note.template.version &&
                type == note.type &&
                createdByName == note.createdByName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
