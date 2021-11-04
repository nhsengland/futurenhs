using System;
using System.ComponentModel.DataAnnotations;

namespace MvcForum.Core.Models.SystemPages
{
    public class SystemPageViewModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
