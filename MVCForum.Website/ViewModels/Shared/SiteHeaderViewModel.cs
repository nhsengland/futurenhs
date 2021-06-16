namespace MvcForum.Web.ViewModels.Shared
{
    using MvcForum.Core.Models.Entities;
    using System.Collections.Generic;
    using MvcForum.Core.Models.General;

    /// <summary>
    /// Defines the SiteHeaderViewModel.
    /// </summary>
    public class SiteHeaderViewModel
    {
        /// <summary>
        /// Gets or sets the CurrentUser (currently logged in user).
        /// </summary>
        public MembershipUser CurrentUser { get; set; }

        public List<NavItemBase> NavigationItems { get; set; }

    }
}