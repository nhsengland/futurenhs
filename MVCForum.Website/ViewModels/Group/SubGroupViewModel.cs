namespace MvcForum.Web.ViewModels.Group
{
    using System.Collections.Generic;
    using Core.Models.Entities;
    using Core.Models.General;

    public class SubGroupViewModel
    {
        public Dictionary<Group, PermissionSet> AllPermissionSets { get; set; }
        public Group ParentGroup { get; set; }
    }
}