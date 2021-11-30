using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using MvcForum.Core.Repositories.Models;

namespace MvcForum.Core.Models.FilesAndFolders
{
    public class FileWriteViewModel
    {
        public Guid FileId { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "The title cannot be more than 1000 characters.")]
        public string Name { get; set; }

        [MaxLength(4000, ErrorMessage = "The description cannot be more than 4000 characters.")]
        public string Description { get; set; }

        public Guid FolderId { get; set; }

        public string Slug { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ModifiedBy { get; set; }

        [Required]
        [Display(Name = "File upload")]
        public HttpPostedFileBase PostedFile { get; set; }

        public int UploadStatus { get; set; }

        /// <summary>
        /// Original filename of uploaded file (rather than meta title entered in form).
        /// </summary>
        [MaxLength(100, ErrorMessage = "The name of the file cannot be more than 100 characters.")]
        public string FileName { get; set; }
        
        public long FileSize { get; set; }
         
        public string FileExtension { get; set; }

        public string FileUrl { get; set; }

        public byte[] BlobHash { get; set; }

        public BreadcrumbsViewModel Breadcrumbs { get; set; }
    }
}