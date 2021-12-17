namespace MvcForum.Web.ViewModels.Member
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Application;
    using MvcForum.Core.Models.Attributes;

    public class MemberFrontEndEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.FirstName")]
        [Required(ErrorMessage = "Please provide a first name")]
        [StringLength(255, ErrorMessage = "The first name must not be greater than 255 characters.")]
        public string FirstName { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Surname")]
        [StringLength(255, ErrorMessage = "The last name must not be greater than 255 characters.")]
        public string Surname { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Pronouns")]
        [StringLength(255, ErrorMessage = "The Pronouns must not be greater than 255 characters.")]
        public string Pronouns { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.EmailAddress")]
        public string Email { get; set; }

        [ValidateFileType("JPEG,JPG,PNG", ErrorMessage = "The selected file must be a JPG or PNG.")]
        [ValidateFileLength(65536, ErrorMessage = "The image file is too large, it must not be greater than 64KB.")]
        [ForumMvcResourceDisplayName("Members.Label.ProfileImage")]
        public HttpPostedFileBase ProfileImage { get; set; }

        public string Avatar { get; set; }

        public bool DisableFileUploads { get; set; }

        public string Initials { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm that all changes are in line with the platform terms and conditions.")]
        public bool HasAgreedToTermsAndConditions { get; set; }

        public string ImageUploadGuidance { get; set; }

        public bool ShowImageUploadGuidance()
        {
            return !string.IsNullOrWhiteSpace(ImageUploadGuidance);
        }

        public bool ShowPronouns()
        {
            return !string.IsNullOrWhiteSpace(Pronouns);
        }
    }
}