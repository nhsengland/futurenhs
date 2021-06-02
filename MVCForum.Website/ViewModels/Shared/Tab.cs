namespace MvcForum.Web.ViewModels.Shared
{
    public class Tab
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; }

        public int Order { get; set; }
        public string Icon { get; set; }
    }
}