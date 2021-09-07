namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class GroupInviteController : Controller
    {

        private readonly IGroupInviteService _inviteService;
        public GroupInviteController(IGroupInviteService inviteService)
        {
            _inviteService = inviteService;
        }

        [HttpGet]
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [Authorize]
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync(string slug, Guid groupId, bool success = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await IsCurrentUserAGroupAdministratorAsync(cancellationToken))
            {
                return RedirectToRoute("GroupUrls", new { slug = slug });
            }

            ViewBag.HideSideBar = true;

            var model = new GroupInviteViewModel
            {
                GroupId = groupId,
                Slug = slug,
                Success = success
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [Authorize]
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync(GroupInviteViewModel model, CancellationToken cancellationToken)
        {
            ViewBag.HideSideBar = true;
            model.Success = false;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var inviteMailAddress = new MailAddress(model.EmailAddress);

            //check if a user is already in the group
            if (await _inviteService.MemberExistsInGroupAsync(model.GroupId, inviteMailAddress, cancellationToken))
            {
                ModelState.AddModelError("EmailExists", "A user with that email address is already a member of this group");
                return View(model);
            }

            //check if a user has is already in the system
            if (await _inviteService.MemberExistsInSystemAsync(inviteMailAddress, cancellationToken))
            {
                ModelState.AddModelError("EmailExists", "A user with that email address is already registered on the platform - you may add them to your group");
                return View(model);
            }

            //check if a user already has an invite for this group
            if (await _inviteService.InviteExistsForGroupAsync(model.GroupId, inviteMailAddress, cancellationToken))
            {
                ModelState.AddModelError("EmailExists", "This email address has already been invited to this group");
                return View(model);
            }
            
            var result = await _inviteService.CreateInviteAsync(model, cancellationToken);
            
            if (result == Guid.Empty)
            {
                ModelState.AddModelError("EmailSendError", "Failed to send email");
                return View(model);
            }

            return RedirectToAction("Create", new { slug = model.Slug, groupId = model.GroupId, success = true});
        }

        private async Task<bool> IsCurrentUserAGroupAdministratorAsync(CancellationToken cancellationToken)
        {
            var currentUsername = System.Web.HttpContext.Current.User.Identity.Name;
            return await _inviteService.IsMemberAdminAsync(currentUsername, cancellationToken);
        }
    }
}