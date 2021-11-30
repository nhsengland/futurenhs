namespace MvcForum.Core.Models
{
    using MvcForum.Core.Models.Entities;
    using System;

    /// <summary>
    /// Used when listing Groups
    /// </summary>
    public class GroupSummary
    {
        public Group Group { get; set; }
        public int MemberCount { get; set; }
        public int DiscussionCount { get; set; }
    }
}