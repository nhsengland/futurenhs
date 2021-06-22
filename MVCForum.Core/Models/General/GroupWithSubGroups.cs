namespace MvcForum.Core.Models.General
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    ///     This is a special domain model and not mapped to the database
    ///     its use to make an efficient call to get a Group with subGroups and only
    ///     used for display purposes. This is due to the database structure and EF getting in a
    ///     mess when we try to map sub Groups by doing a join on itself (Group table)
    /// </summary>
    public partial class GroupWithSubGroups
    {
        public Group Group { get; set; }
        public IEnumerable<Group> SubGroups { get; set; }
    }
}