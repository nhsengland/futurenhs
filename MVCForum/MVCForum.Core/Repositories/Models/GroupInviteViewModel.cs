namespace MvcForum.Core.Repositories.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GroupInviteViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Required]
        [Compare("EmailAddress", ErrorMessage = "Please provide matching email addresses")]
        [Display(Name = "Repeat email address")]
        public string ConfirmEmailAddress { get; set; }

        [Required]
        public string Slug { get; set; }

        public Guid? GroupId { get; set; }

        public bool IsDeleted { get; set; }

        public bool Success { get; set; }

    }
}
