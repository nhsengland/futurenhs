namespace MvcForum.Web.ViewModels.Shared
{
    using MvcForum.Core.Models.Entities;

    /// <summary>
    /// Defines the SiteHeaderViewModel.
    /// </summary>
    public class SiteHeaderViewModel
    {
        /// <summary>
        /// Gets or sets the CurrentUser (currently logged in user).
        /// </summary>
        public MembershipUser CurrentUser { get; set; }

        public PageViewModel PageHeader { get; set; }

    }
}