namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Constants;
    using MvcForum.Core.Constants.UI;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.General;
    using MvcForum.Web.ViewModels.Shared;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Controller for layout components.
    /// </summary>
    public class LayoutController : Controller
    {
        /// <summary>
        /// Gets or sets the _membershipService for interactions with membership data.
        /// </summary>
        private IMembershipService _membershipService { get; set; }

        private IGroupService _groupService { get; set; }

        private MembershipUser LoggedOnReadOnlyUser;

        /// <summary>
        /// Constructs a new instance of the layout controller.
        /// </summary>
        /// <param name="membershipService"></param>
        public LayoutController(IMembershipService membershipService, IGroupService groupService)
        {
            _membershipService = membershipService;
            _groupService = groupService;
            LoggedOnReadOnlyUser = membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);

        }

        /// <summary>
        /// Renders the site header partial view.
        /// </summary>
        /// <returns>Partial view for the site header.</returns>
        public ActionResult SiteHeader()
        {
            var model = new SiteHeaderViewModel
            {
                NavigationItems = GetMenuNavigation(),
                CurrentUser = _membershipService.GetUser(User.Identity.Name)
            };
            return PartialView("_SiteHeader", model);
        }

        /// <summary>
        /// Method to get the menu navigation items.
        /// </summary>
        /// <returns>List of navigation items <see cref="NavItemBase"/>.</returns>
        [ChildActionOnly]
        private List<LinkGroup> GetMenuNavigation()
        {
            List<LinkGroup> navItems = new List<LinkGroup>() {
                new LinkGroup { IconTheme=Themes.FILL_THEME_8, Icon = Icons.Group, Name = "Groups", Url="/", Order = 1, BorderTheme = Themes.BORDER_8 },
                    };

            return navItems;
        }

        public PartialViewResult SideNavigation()
        {
            List<Link> model = new List<Link> {
                new Link { Name = "Groups", Url= Url.Action("Index", "Home"), Icon=Icons.Group, IconTheme=Themes.FILL_THEME_8 },
                };

            return PartialView("_SideBar", model);
        }


    }
}