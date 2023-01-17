using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record FileData
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime? CreatedAtUtc { get; init; }
        public Guid CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string CreatorSlug { get; set; }
        public DateTime? ModifiedAtUtc { get; init; }
        public long? Size { get; init; }
        public Guid? ModifierId { get; set; }
        public string? ModifierName { get; set; }
        public string ModifierSlug { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

    }
}
