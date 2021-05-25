namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Web.ViewModels.Shared;
    using System.Web.Mvc;

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
            SiteHeaderViewModel model = new SiteHeaderViewModel()
            {
                CurrentUser = _membershipService.GetUser(User.Identity.Name)
            };

            return PartialView("_SiteHeader", model);
        }
    }
}