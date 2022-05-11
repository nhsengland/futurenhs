namespace Umbraco9ContentApi.Core.Models.Content
{
    public class ContentModel
    {
        public virtual ContentModelItem Item { get; set; }
        public virtual Dictionary<string, object> Content { get; set; }
    }
}
