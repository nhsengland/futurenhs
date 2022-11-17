namespace FileServer.Models
{
    public sealed record UserFileMetadata
    {
        public Guid FileId { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }

        public string? GroupName { get; init; }

        public string? Name { get; init; }
        public string? FileVersion { get; init; }
        public string? Extension { get; init; }

        public string? BlobName { get; init; }
        public byte[]? ContentHash { get; init; }

        public string? OwnerUserName { get; init; }

        public ulong? SizeInBytes { get; init; }
        public DateTimeOffset? LastWriteTimeUtc { get; init; }
        public FileServer.Models.File AsFile() => FileServer.Models.File.FromId(FileId, FileVersion);
    }
}
