using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record FolderContentsData
    {
        public Guid Id { get; init; }
        public string Type { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string? CreatedAtUtc { get; init; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySlug { get; set; }
        public string? ModifiedAtUtc { get; init; }
        public Guid? ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedBySlug { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }

    }
}
