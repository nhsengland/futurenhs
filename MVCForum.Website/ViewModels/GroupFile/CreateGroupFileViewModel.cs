using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcForum.Web.ViewModels.GroupFile
{
    public class CreateGroupFileViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid FolderId { get; set; }
    }
}