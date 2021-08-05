using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcForum.Web.ViewModels.Shared
{
    public class GroupHeaderViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Colour { get; set; }
        public TabViewModel HeaderTabs { get; set; }
    }
}