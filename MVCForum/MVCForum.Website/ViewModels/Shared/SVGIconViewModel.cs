namespace MvcForum.Web.ViewModels.Shared
{
    public class SVGIconViewModel
    {
        public string Id { get; set; } 
        public string CssClasses { get; set; }

        public SVGIconViewModel(string Id, string CssClasses)
        {
            this.Id = Id;
            this.CssClasses = CssClasses;
        }
    }
}