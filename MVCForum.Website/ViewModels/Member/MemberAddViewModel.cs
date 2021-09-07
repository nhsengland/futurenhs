﻿namespace MvcForum.Web.ViewModels.Member
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Application;
    using Core.Models.Entities;
    using Core.Models.Enums;

    public class MemberAddViewModel
    {

        [Required]
        [EmailAddress]
        [ForumMvcResourceDisplayName("Members.Label.EmailAddress")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string Surname { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 10)]
        [DataType(DataType.Password)]
        [ForumMvcResourceDisplayName("Members.Label.Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [ForumMvcResourceDisplayName("Members.Label.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int MinPasswordLength { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.UserIsApproved")]
        public bool IsApproved { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Comment")]
        public string Comment { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Roles")]
        public string[] Roles { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }
        public string SpamAnswer { get; set; }
        public string ReturnUrl { get; set; }
        public string SocialProfileImageUrl { get; set; }
        public string UserAccessToken { get; set; }
        public LoginType LoginType { get; set; }
    }
}