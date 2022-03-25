using FutureNHS.Api.DataAccess.Models;
using Newtonsoft.Json;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public record GroupDto : BaseData
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string? StrapLine { get; init; }
        public Guid? ThemeId { get; init; }
        public Guid? ImageId { get; init; }
        public bool IsPublic { get; init; }
        public string Slug { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public Guid? CreatedBy { get; init; }
        public DateTime ModifiedAtUtc { get; init; }
        public Guid? ModifiedBy { get; init; }
        public bool IsDeleted { get; init; }
    }
}
