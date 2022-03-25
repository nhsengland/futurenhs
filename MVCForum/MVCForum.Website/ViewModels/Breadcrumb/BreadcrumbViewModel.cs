namespace MvcForum.Web.ViewModels.Breadcrumb
{
    using System.Collections.Generic;
    using Core.Models.Entities;

    public class BreadcrumbViewModel
    {
        public List<Group> Groups { get; set; }
        public Topic Topic { get; set; }
        public Group Group { get; set; }
    }
}