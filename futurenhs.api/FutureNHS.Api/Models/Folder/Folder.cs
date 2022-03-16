using System.ComponentModel.DataAnnotations;

namespace FutureNHS.Api.Models.Folder
{
    public sealed class Folder
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }
    }
}
