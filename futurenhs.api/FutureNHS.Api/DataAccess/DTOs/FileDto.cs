namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record FileDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public long FileSizeBytes { get; set; }
        public string FileExtension { get; set; }
        
        public string VersionId { get; set; }
        public string BlobName { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAtUTC { get; set; }
        public Guid? ParentFolder { get; set; }
        public Guid FileStatus { get; set; }
        public byte[] BlobHash { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
