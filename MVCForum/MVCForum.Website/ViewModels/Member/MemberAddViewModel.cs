namespace MvcForum.Web.ViewModels.Member
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Application;
    using Core.Models.Entities;
    using Core.Models.Enums;

    public class MemberAddViewModel
    {
        [Required(ErrorMessage = "Please provide a valid email address")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please provide your first name")]
        [Display(Name = "First name")]        
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Your password must be at least 10 characters long")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Your password must be at least 10 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Your passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int MinPasswordLength { get; set; }

        [Display(Name = "User is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Roles")]
        public string[] Roles { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }
        public string SpamAnswer { get; set; }
        public string ReturnUrl { get; set; }
        public string SocialProfileImageUrl { get; set; }
        public string UserAccessToken { get; set; }
        public LoginType LoginType { get; set; }
    }
}