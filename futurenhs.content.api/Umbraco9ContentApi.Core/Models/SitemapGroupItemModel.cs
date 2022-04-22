namespace Umbraco9ContentApi.Core.Models
{
    /// <summary>
    /// The SitemapGroupItemModel for front end website structure.
    /// </summary>
    public class SitemapGroupItemModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the edited at.
        /// </summary>
        public DateTime EditedAt { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; }

        public SitemapGroupItemModel(Guid id, string name, string title, Guid parentId, DateTime createdAt, DateTime editedAt, int level)
        {
            Id = id;
            Name = name;
            Title = title;
            ParentId = parentId;
            CreatedAt = createdAt;
            EditedAt = editedAt;
            Level = level - 2; // we minus two as the cms structure doesn't reflect the group site structure otherwise.
        }
    }
}
