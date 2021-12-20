using MvcForum.Core.Utilities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcForum.Core.Models.Entities
{
    [Table("Image")]
    public class Image
    {
        public Image()
        {
            this.Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }

        public string FileName { get; set; }

        public int FileSizeBytes { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string MediaType { get; set; }

        public bool IsDeleted { get; set; }
    }
}
