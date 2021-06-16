namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Constants;
    using MvcForum.Core.Constants.UI;
    using MvcForum.Core.Interfaces.Services;
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

        /// <summary>
        /// Constructs a new instance of the layout controller.
        /// </summary>
        /// <param name="membershipService"></param>
        public LayoutController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
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
        private List<NavItemBase> GetMenuNavigation()
        {
            List<NavItemBase> navItems = new List<NavItemBase>() {
                new Link { IconTheme=Themes.FILL_THEME_8, Icon = Icons.HomeOutline, Name = "Home", Url="/", Order = 1, BorderTheme = Themes.BORDER_8 },
                new LinkGroup { IconTheme = Themes.FILL_THEME_10, Icon=Icons.ForumOutline, Name = "Forum", Order = 10, BorderTheme=Themes.BORDER_10 , ChildItems = new List<Link> { new Link { Url="/" } } },
                new Link { IconTheme=Themes.FILL_THEME_9, Icon = Icons.Star, Order = 15, Name = "Favourites", Url = "/", BorderTheme=Themes.BORDER_9 }
            };

            RouteData routeData = RouteTable.Routes.GetRouteData(HttpContext);
            if ((string)routeData.Values["controller"] == "Group" && (string)routeData.Values["action"] == "Show")
            {
                var slug = routeData.Values["slug"];

                navItems.Add(new LinkGroup()
                {
                    Name = "Group",
                    Icon = Icons.Group,
                    IconTheme = Themes.FILL_THEME_11,
                    BorderTheme = Themes.BORDER_11,
                    Order = 5,
                    ChildItems = new List<Link>()
                    {
                        new Link { Name = "Home", Url = Url.RouteUrl("GroupUrls", new { slug = slug, tab = UrlParameter.Optional }) },
                        new Link { Name = "Members", Url = Url.RouteUrl("GroupUrls", new { slug = slug, tab= Constants.GroupMembersTab }) },
                        new Link { Name = "Forum", Url = Url.RouteUrl("GroupUrls", new { slug = slug, tab= Constants.GroupForumTab }) }
                    }
                });
            }

            return navItems;
        }

        public PartialViewResult SideNavigation()
        {
            List<Link> model = new List<Link> { 
                new Link { Name = "Home", Url="/", Icon=Icons.HomeOutline, IconTheme=Themes.FILL_THEME_8 },
                new Link { Name = "Groups", Url= Url.Action("Index", "Group"), Icon=Icons.Group, IconTheme=Themes.FILL_THEME_11 }
            };


            return PartialView("_SideBar", model);
        }
    }
}