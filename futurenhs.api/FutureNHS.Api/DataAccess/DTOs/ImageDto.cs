using FutureNHS.Api.DataAccess.Models;
using Newtonsoft.Json;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public record ImageDto: BaseData
    {
        public Guid Id { get; init; }
        [JsonIgnore]
        public long FileSizeBytes { get; init; }
        public int Height { get; init; }
        public int Width { get; init; }
        public string FileName { get; init; }
        public string MediaType { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public Guid CreatedBy { get; init; }
        [JsonIgnore]
        public bool IsDeleted { get; init; }
    }
}
