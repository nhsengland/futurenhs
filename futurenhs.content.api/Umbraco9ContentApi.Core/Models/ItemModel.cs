namespace Umbraco9ContentApi.Core.Models
{
    public class ItemModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? UrlSegment { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EditedAt { get; set; }
        public string? ContentType { get; set; }
        public string? Locale { get; set; }
        public string? Url { get; set; }
    }
}
