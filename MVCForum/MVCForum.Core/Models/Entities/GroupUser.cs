namespace MvcForum.Core.Models.Entities
{
    using System;
    using Interfaces;
    using Utilities;

    public partial class GroupUser : IBaseEntity
    {
        public GroupUser()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public virtual Group Group { get; set; }
        public virtual MembershipUser User { get; set; }
        public bool Approved { get; set; }
        public bool Rejected { get; set; }
        public bool Locked { get; set; }
        public bool Banned { get; set; }
        public virtual MembershipRole Role { get; set; }
        public virtual MembershipUser ApprovingUser { get; set; }
        public DateTime RequestToJoinDate { get; set; }
        public DateTime? ApprovedToJoinDate { get; set; }
        public string RequestToJoinReason { get; set; }
        public string LockReason { get; set; }
        public string BanReason { get; set; }
    }
}
