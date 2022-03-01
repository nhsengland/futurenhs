namespace Umbraco9ContentApi.Core.Models
{
    /// <summary>
    /// The GroupSiteMapItemViewModel for front end website structure.
    /// </summary>
    public class GroupSitemapItemViewModel
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public Guid ParentId { get; set; }

        public Guid Id { get; set; }

        public string Url { get; set; }

        public string Date { get; set; }

        public int Level { get; set; }
    }
}
