namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Web.Common.Controllers;
    /// <summary>
    /// Overrides the default homepage view to redirect to the swagger page.
    /// </summary>
    public class DefaultRenderController : IRenderController
    {
        /// <summary>
        /// Default route.
        /// </summary>
        /// <returns>A redirect to the swagger page.</returns>
        public IActionResult Index() => new RedirectResult("/swagger", true);
    }
}