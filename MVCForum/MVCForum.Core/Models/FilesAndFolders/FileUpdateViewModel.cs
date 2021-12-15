namespace MvcForum.Core.Models.FilesAndFolders
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public sealed class FileUpdateViewModel
    {
        public Guid FileId { get; set; }

        public string OriginalFileTitle { get; set; }

        [Required(ErrorMessage = "Please provide a file name")]
        [MaxLength(45, ErrorMessage = "The file title must not be more than 45 characters.")]
        public string Name { get; set; }

        [MaxLength(150, ErrorMessage = "The file description must not be more than 150 characters.")]
        public string Description { get; set; }

        public Guid FolderId { get; set; }

        public string GroupSlug { get; set; }

        public Guid ModifiedBy { get; set; }

        public BreadcrumbsViewModel Breadcrumbs { get; set; }
    }
}
