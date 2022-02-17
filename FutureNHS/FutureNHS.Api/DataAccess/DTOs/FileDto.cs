namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record FileDto
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public string FileName { get; init; }
        public int FileSizeBytes { get; init; }
        public string FileExtension { get; init; }
        public string BlobName { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime CreatedAtUTC { get; init; }
        public Guid? ModifiedBy { get; init; }
        public DateTime? ModifiedAtUTC { get; init; }
        public Guid? ParentFolder { get; init; }
        public Guid FileStatus { get; init; }
        public byte[] BlobHash { get; init; }
        public bool IsDeleted { get; init; }
        public byte[] RowVersion { get; set; }
    }
}
