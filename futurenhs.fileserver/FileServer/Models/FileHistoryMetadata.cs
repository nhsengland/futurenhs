using System.Data;

namespace FileServer.Models
{
    public sealed record FileHistoryMetadata
    {
        public Guid FileId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string FileName { get; init; }
        public long FileSizeBytes { get; init; }
        public string FileExtension { get; init; }
        public string BlobName { get; init; }
        public Guid ModifiedBy { get; init; }
        public DateTime? ModifiedAtUtc { get; init; } //Type should be DateTimeOffset ?
        public Guid FileStatus { get; init; }
        public byte[] BlobHash { get; init; } 
        public string VersionId { get; init; }
        public bool IsDeleted { get; init; }
        public byte[] RowVersion { get; init; }
        
    }
}
