using MvcForum.Core.Constants;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.SystemPages;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcForum.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.AdminRoleName)]
    public class AdminSystemPagesController : Controller
    {
        private readonly ISystemPagesService _systemPagesService;
        public AdminSystemPagesController(ISystemPagesService systemPagesService)
        {
            _systemPagesService = systemPagesService;
        }
        // GET: Admin/AdminSystemPages
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var pages = await _systemPagesService.GetAllSystemPages(cancellationToken);
            return View(pages);
        }

        public ActionResult Create()
        {
            return View(new SystemPageWriteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SystemPageWriteViewModel pageModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) 
                return View(pageModel);

            if (!_systemPagesService.IsValidSlug(pageModel.Slug))
            {
                ModelState.AddModelError("Slug", "The slug provided is not valid, it must not contain any special characters or spaces");
                return View(pageModel);
            }

            var result = await _systemPagesService.CreateSystemPage(pageModel, cancellationToken);

            if (result.Response == ResponseType.Success) 
                return RedirectToAction("Index");
            
            switch (result.Response)
            {
                case ResponseType.AlreadyExists:
                    ModelState.AddModelError("Slug", "The slug provided already exists, it must be unique");
                    break;
                case ResponseType.Error:
                    ModelState.AddModelError("", "Sorry there was an error, please try again");
                    break;
                case ResponseType.PermissionDenied:
                    ModelState.AddModelError("", "You do not have permission to do this");
                    break;
                default:
                    ModelState.AddModelError("", "Sorry there was an error, please try again");
                    break;
            }

            return View(pageModel);

        }


        public async Task<ActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var page = await _systemPagesService.GetSystemPageById(id, cancellationToken);

            if (page == null) 
                return new HttpNotFoundResult("Page not found");

            var writeModel = new SystemPageWriteViewModel
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Content = page.Content
            };
            return View(writeModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SystemPageWriteViewModel pageModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) 
                return View(pageModel);

            if (!_systemPagesService.IsValidSlug(pageModel.Slug))
            {
                ModelState.AddModelError("Slug", "The slug provided is not valid, it must not contain any special characters or spaces, only letters or hyphens");
                return View(pageModel);
            }

            var result = await _systemPagesService.UpdateSystemPage(pageModel, cancellationToken);

            if (result.Response == ResponseType.Success) 
                return RedirectToAction("Index");

            switch (result.Response)
            {
                case ResponseType.DoesntExist:
                    ModelState.AddModelError("", "We couldn't find the page to update");
                    break;
                case ResponseType.AlreadyExists:
                    ModelState.AddModelError("Slug", "The slug provided already exists, it must be unique");
                    break;
                case ResponseType.Error:
                    ModelState.AddModelError("", "Sorry there was an error, please try again");
                    break;
                case ResponseType.PermissionDenied:
                    ModelState.AddModelError("", "You do not have permission to do this");
                    break;
                default:
                    ModelState.AddModelError("", "Sorry there was an error, please try again");
                    break;
            }

            return View(pageModel);

        }

        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _systemPagesService.DeleteSystemPage(id, cancellationToken);

            if (result.Response == ResponseType.Success) 
                return RedirectToAction("Index");

            switch (result.Response)
            {
                case ResponseType.DoesntExist:
                    ModelState.AddModelError("", "We couldn't find the page to delete");
                    break;
                case ResponseType.Error:
                    ModelState.AddModelError("", "Sorry there was an error, please try again");
                    break;
                case ResponseType.PermissionDenied:
                    ModelState.AddModelError("", "You do not have permission to do this");
                    break;
                default:
                    ModelState.AddModelError("", "Sorry there was an error, please try again");
                    break;
            }

            var page = await _systemPagesService.GetSystemPageById(id, cancellationToken);

            var writeModel = new SystemPageWriteViewModel
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Content = page.Content
            };
            return View("Edit", writeModel);
        }
    }
}