using System;

namespace MvcForum.Core.Repositories.Models
{
    public class ImageViewModel
    {
        public ImageViewModel(string source, string altText)
        {
            Source = source;
            AltText = altText;
        }

        public string Source { get; }
        public string AltText { get; }

        public Guid Id { get; set; }
        public string FileName { get; set; }
        public int FileSizeBytes { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string MediaType { get; set; }
    }
}
