namespace MvcForum.Web.ViewModels.Registration
{
    using System.ComponentModel.DataAnnotations;
    using Application;

    public class LogOnViewModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}