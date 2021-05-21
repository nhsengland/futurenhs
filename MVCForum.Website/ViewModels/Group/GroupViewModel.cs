using MvcForum.Core.Models.Enums;

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
    }


}