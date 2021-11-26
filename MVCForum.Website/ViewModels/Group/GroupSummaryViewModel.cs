using MvcForum.Core.Models.General;
using System;

namespace MvcForum.Web.ViewModels.Group
{
    public sealed class GroupSummaryViewModel
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string NiceUrl { get; set; }
        public int MemberCount { get; set; }
        public int DiscussionCount { get; set; }
        public PermissionSet Permissions { get; set; }
    }
}