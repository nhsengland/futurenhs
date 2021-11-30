using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcForum.Web.ViewModels.Shared;

namespace MvcForum.Web.ViewModels.Group
{
    public class GroupsLandingViewModel
    {
        public string CurrentTab { get; set; }

        public GroupHeaderViewModel Header { get; set; }
    }
}