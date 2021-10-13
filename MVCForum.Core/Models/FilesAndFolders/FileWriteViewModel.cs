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
        public string Name { get; set; }

        [Required]
        [UIHint(Constants.Constants.EditorType)]
        [AllowHtml]
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
        public string FileName { get; set; }
        
        public long FileSize { get; set; }
         
        public string FileExtension { get; set; }

        public string FileUrl { get; set; }

        public byte[] BlobHash { get; set; }

        public BreadcrumbsViewModel Breadcrumbs { get; set; }
    }
}