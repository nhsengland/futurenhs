namespace MvcForum.Web.ViewModels.Group
{
    using System.Collections.Generic;
    using Core.Models;
    using Core.Models.Entities;
    using Core.Models.General;

    public class GroupListViewModel
    {
        public Dictionary<Group, PermissionSet> AllPermissionSets { get; set; }
    }

    public class MyGroupListViewModel
    {
        public List<GroupUser> MyGroups { get; set; }
    }

    public class GroupListSummaryViewModel
    {
        public Dictionary<GroupSummary, PermissionSet> AllPermissionSets { get; set; }
    }
}