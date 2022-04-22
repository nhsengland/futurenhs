namespace Umbraco9ContentApi.Core.Models
{
    public class ContentModel
    {
        public ItemModel? Item { get; set; }
        public Dictionary<string, object>? Content { get; set; }
    }
}
