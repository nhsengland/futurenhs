using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcForum.Core.Models.SystemPages
{
    public class SystemPageWriteViewModel
    {
        private string _slug;
        public Guid Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Slug must be less than 30 characters and more than 5 characters")]
        public string Slug
        {
            get => _slug?.ToLower();
            set => _slug = value.ToLower();
        }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be less than 100 characters and more than 5 characters")]
        public string Title { get; set; }

        [Required]
        [StringLength(10000, MinimumLength = 50, ErrorMessage = "Content must be less than 10,000 characters and more than 50 characters")]
        [DataType(DataType.MultilineText)]
        [UIHint(Constants.Constants.EditorType)]
        [AllowHtml]
        public string Content { get; set; }
    }
}
