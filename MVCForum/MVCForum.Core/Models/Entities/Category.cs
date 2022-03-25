namespace MvcForum.Core.Models.Entities
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    ///     Category class is a way to segment topics within a group
    /// </summary>
    public class Category : ExtendedDataEntity, IBaseEntity
    {
        public Category()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int SortOrder { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual Group Group { get; set; }
        public virtual IList<Topic> Topics { get; set; }

    }
}