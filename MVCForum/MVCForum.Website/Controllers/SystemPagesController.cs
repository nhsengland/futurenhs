namespace MvcForum.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MvcForum.Core.Interfaces.Services;
    using System.Web.Mvc;
    using System;

    public class SystemPagesController : Controller
    {
        private readonly ISystemPagesService _systemPagesService;

        public SystemPagesController(ISystemPagesService systemPagesService)
        {
            _systemPagesService = systemPagesService;
        }

        [ActionName("Show")]
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public async Task<ActionResult> ShowAsync(string slug, CancellationToken cancellationToken)
        {
            var page = await _systemPagesService.GetSystemPageBySlug(slug.ToLower(), cancellationToken);
            if (page != null)
            {
                return View("Show", page);
            }

            return new HttpNotFoundResult("Page not found");
        }

    }
}