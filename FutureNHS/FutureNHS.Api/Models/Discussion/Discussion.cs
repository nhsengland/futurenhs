using System.ComponentModel.DataAnnotations;

namespace FutureNHS.Api.Models.Discussion
{
    public class Discussion
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsSticky { get; set; }
    }
}
