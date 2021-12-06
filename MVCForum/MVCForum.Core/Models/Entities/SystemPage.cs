namespace MvcForum.Core.Models.Entities
{
    using System;
    using Interfaces;
    using Utilities;

    public class SystemPage : IBaseEntity
    {
        public SystemPage()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
    }
}
