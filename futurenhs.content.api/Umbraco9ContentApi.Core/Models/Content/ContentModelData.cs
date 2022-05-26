namespace Umbraco9ContentApi.Core.Models.Content
{
    public class ContentModelData
    {
        public virtual ContentModelItemData Item { get; set; }
        public virtual Dictionary<string, object> Content { get; set; }
    }
}
