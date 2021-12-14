using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ImageProcessor.Common.Exceptions;
using Microsoft.Ajax.Utilities;
using MvcForum.Core.Repositories.Repository.Interfaces;
using MvcForum.Core.Services;
using MvcForum.Web.Attributes;

namespace MvcForum.Web.Controllers.ApiControllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [ApiAuthorize]
        public async Task<JsonResult> UserInfo()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var userInfo = await _userRepository.GetUser(userName);
                return Json(userInfo, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [Authorize]
        [HttpGet]
        public ActionResult LogOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            Response.StatusCode = 200;
            return RedirectToAction("Index", "Home");
        }
    }
}
