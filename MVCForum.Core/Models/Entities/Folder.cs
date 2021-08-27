using MvcForum.Core.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcForum.Core.Models.Entities
{
    public class Folder
    {
        public Folder()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentFolder { get; set; }
        public int FileCount { get; set; }
        public Guid AddedBy { get; set; }
        public Guid ParentGroup { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
