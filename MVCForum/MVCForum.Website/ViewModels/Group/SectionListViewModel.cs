namespace MvcForum.Web.ViewModels.Group
{
    using System.Collections.Generic;
    using Core.Models;
    using Core.Models.Entities;
    using Core.Models.General;

    public class SectionListViewModel
    {
        public Section Section { get; set; }
        public Dictionary<GroupSummary, PermissionSet> AllPermissionSets { get; set; }
    }
}