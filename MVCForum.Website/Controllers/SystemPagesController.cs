//-----------------------------------------------------------------------
// <copyright file="FolderController.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Interfaces.Services;
    using System.Web.Mvc;

    public class SystemPagesController : Controller
    {
        private readonly ISystemPagesService _systemPagesService;

        public SystemPagesController(ISystemPagesService systemPagesService)
        {
            _systemPagesService = systemPagesService;
        }
        public async Task<ActionResult> Show(string slug, CancellationToken cancellationToken)
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