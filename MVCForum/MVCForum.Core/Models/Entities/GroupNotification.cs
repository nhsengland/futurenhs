namespace MvcForum.Core.Models.Entities
{
    using System;
    using Interfaces;
    using Utilities;

    public partial class GroupNotification : IBaseEntity
    {
        public GroupNotification()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public virtual Group Group { get; set; }
        public virtual MembershipUser User { get; set; }
    }
}
