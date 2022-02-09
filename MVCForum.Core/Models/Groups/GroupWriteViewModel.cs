namespace MvcForum.Core.Models.Groups
{
    using ValidateFileType = MvcForum.Core.Models.Attributes.ValidateFileTypeAttribute;
    using ValidateFileLength = MvcForum.Core.Models.Attributes.ValidateFileLengthAttribute;
    using MvcForum.Core.Utilities;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    public sealed class GroupWriteViewModel
    {
        public GroupWriteViewModel()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }

        [DisplayName("Group name")]
        [Required(ErrorMessage = "Please provide a name for your group")]
        [StringLength(255, ErrorMessage = "The group name must not be greater than 255 characters.")]
        public string Name { get; set; }

        [DisplayName("Strap line")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "The strap line must not be greater than 1000 characters.")]
        public string Description { get; set; }


        [DisplayName("Group introduction")]
        [Required(ErrorMessage = "Please provide an introduction to your group.")]
        [StringLength(4000, ErrorMessage = "The group introduction must not be greater than 4000 characters.")]
        public string Introduction { get; set; }

        [DisplayName("Group rules")]
        [StringLength(4000, ErrorMessage = "The group rules must not be greater than 4000 characters.")]
        public string AboutUs { get; set; }

        [DisplayName("Logo")]
        [ValidateFileType("JPEG,JPG,PNG", ErrorMessage = "The selected file must be a JPG or PNG.")]
        [ValidateFileLength(5 * 1024 * 100, ErrorMessage = "The logo file is too large.  It must not be greater then 500KB.")]
        public HttpPostedFileBase Files { get; set; }

        public bool PublicGroup { get; set; }
        public string Image { get; set; }

        public string Slug { get; set; }
    }
}