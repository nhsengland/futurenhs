using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcForum.Web.ViewModels.GroupFile
{
    public class CreateGroupFileViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public Guid FolderId { get; set; }
    }
}