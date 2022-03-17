using System.Text.Json.Serialization;

namespace FutureNHS.Api.DataAccess.Models
{
    public record BaseData
    {
        [JsonIgnore]
        public byte[]? RowVersion { get; set; }
    }
}
