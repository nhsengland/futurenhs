using System.ComponentModel.DataAnnotations;

namespace FutureNHS.Api.Models.Comment
{
    public sealed class Comment
    {
        [Required]
        public string Content { get; set; }

    }
}
