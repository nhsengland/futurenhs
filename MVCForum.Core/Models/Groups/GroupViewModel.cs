namespace MvcForum.Core.Models.Groups
{
    using System;

    public sealed class GroupViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Introduction { get; set; }
        public string AboutUs { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public bool PublicGroup { get; set; }
        public bool IsDeleted { get; set; }
    }
}