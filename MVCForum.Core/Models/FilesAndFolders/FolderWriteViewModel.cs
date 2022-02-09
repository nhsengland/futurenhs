using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Repositories.Models;

namespace MvcForum.Core.Models.FilesAndFolders
{
    public class FolderWriteViewModel
    {
        public string Slug { get; set; }
        public Guid? FolderId { get; set; }

        public string OriginalFolderName { get; set; }

        [Required(ErrorMessage = "Please provide a folder title")]
        [DisplayName("Folder title")]
        [MaxLength(1000, ErrorMessage = "The folder title cannot be more than 1000 characters.")]
        public string FolderName { get; set; }

        [MaxLength(4000, ErrorMessage = "The folder description cannot be more than 4000 characters.")]
        public string Description { get; set; }
        public Guid AddedBy { get; set; }
        public Guid? ParentFolder { get; set; }
        [Required]
        public Guid ParentGroup { get; set; }
        public  bool IsDeleted { get; set; }
        public BreadcrumbsViewModel Breadcrumbs { get; set; }
    }
}
