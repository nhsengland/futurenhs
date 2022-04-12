namespace MvcForum.Web.ViewModels.Registration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Application;

    public class ResetPasswordViewModel
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Your password must be at least 10 characters long")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Your password must be at least 10 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}