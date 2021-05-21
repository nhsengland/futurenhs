using System;
using System.Web.Mvc;
using MvcForum.Core.Models.Enums;

namespace MvcForum.Web.ViewModels.Group
{
    using System.Collections.Generic;
    using Core.Models.Entities;
    using Core.Models.General;
    using Topic;

    public class GroupUserViewModel
    {

        public GroupUser GroupUser { get; set; }

        public GroupUserStatus GroupUserStatus { get; set; }

        public IEnumerable<SelectListItem> RoleSelectList { get; set; }
        public MembershipRole MemberRole { get; set; }

    }


}