namespace MvcForum.Web.ViewModels.Group
{
    using Core.Models.Entities;
    using MvcForum.Core.Models.Enums;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class GroupUserViewModel
    {

        public GroupUser GroupUser { get; set; }

        public GroupUserStatus GroupUserStatus { get; set; }

        public IEnumerable<SelectListItem> RoleSelectList { get; set; }
        public MembershipRole MemberRole { get; set; }

    }
}