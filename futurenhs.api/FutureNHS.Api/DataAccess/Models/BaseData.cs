using System.Text.Json.Serialization;

namespace FutureNHS.Api.DataAccess.Models
{
    public class BaseData
    {
        [JsonIgnore]
        public byte[] RowVersion { get; init; }
    }
}
