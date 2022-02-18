using System.ComponentModel.DataAnnotations;

namespace FutureNHS.Api.Models.File
{
    public sealed class File
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [Required]
        [MaxLength(4000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(256)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(10)]
        public string FileExtension { get; set; }

        [Required]
        public int FileSizeInBytes { get; set; }

        [Required]
        [MaxLength(42)]
        public string BlobName { get; set; }
    }
}
