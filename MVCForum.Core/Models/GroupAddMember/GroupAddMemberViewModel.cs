using MvcForum.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace MvcForum.Core.Models.GroupAddMember
{
    public sealed class GroupAddMemberViewModel
    {
        [Required(ErrorMessage = "Please provide a valid email address")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        [Display(Name = "New member email address")]
        public string Email { get; set; }

        [Required]
        public string Slug { get; set; }

        public ResponseType Response { get; set; }
    }
}
