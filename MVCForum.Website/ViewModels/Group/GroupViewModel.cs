using System;
using MvcForum.Core.Models.Enums;
using MvcForum.Web.ViewModels.Shared;

namespace MvcForum.Web.ViewModels.Group
{
    using System.Collections.Generic;
    using Core.Models.Entities;
    using Core.Models.General;
    using Topic;

    public class GroupViewModel
    {
        public List<TopicViewModel> Topics { get; set; }
        public PermissionSet Permissions { get; set; }
        public Group Group { get; set; }
        public IEnumerable<GroupUserViewModel> GroupUsers { get; set; }
        public GroupListViewModel SubGroups { get; set; }
        public TabViewModel Tabs { get; set; }
        public string Tab { get; set; }
        public MembershipUser User { get; set; }
        public bool IsSubscribed { get; set; }
        public GroupUserStatus GroupUserStatus { get; set; }
        public MembershipRole GroupUserRole {get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public int PostCount { get; set; }

        // Topic info
        public Topic LatestTopic { get; set; }

        public int TopicCount { get; set; }

        // Misc
        public bool ShowUnSubscribedLink { get; set; }

        public Guid? Folder { get; set; }

        public ActionMenuModel ActionMenu { get; set; }
}
  


}