namespace MvcForum.Web.ViewModels.Registration
{
    using System.ComponentModel.DataAnnotations;
    using Application;

    public class ForgotPasswordViewModel
    {
        [Display(Name = "Enter your email address")]
        [Required(ErrorMessage = "Please provide a valid email address")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string EmailAddress { get; set; }
    }
}