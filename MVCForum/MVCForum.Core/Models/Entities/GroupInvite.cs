namespace MvcForum.Core.Models.Entities
{
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Utilities;
    using System;

    public sealed class GroupInvite : IBaseEntity
    {
        public GroupInvite()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public bool IsDeleted { get; set; }
        public Guid GroupId { get; set; }
        
    }
}
