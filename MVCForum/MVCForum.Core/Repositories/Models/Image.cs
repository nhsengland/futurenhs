using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Models
{
    public class Image
    {
        public Image(string source, string altText)
        {
            Source = source;
            AltText = altText;
        }

        public string Source { get; }
        public string AltText { get; }
    }
}
