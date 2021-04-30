namespace MvcForum.Core.Models
{
    using Entities;

    /// <summary>
    /// Used when listing Groups
    /// </summary>
    public class GroupSummary
    {
        public Group Group { get; set; }
        public int TopicCount { get; set; }
        public int PostCount { get; set; }
        public Topic MostRecentTopic { get; set; }
    }
}