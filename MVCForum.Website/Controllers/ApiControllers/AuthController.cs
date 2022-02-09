﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using ImageProcessor.Common.Exceptions;
using Microsoft.Ajax.Utilities;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Repositories.Repository.Interfaces;
using MvcForum.Core.Services;
using MvcForum.Web.Attributes;

namespace MvcForum.Web.Controllers.ApiControllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggingService _loggingService;

        public AuthController(IUserRepository userRepository, ILoggingService loggingService)
        {
            _userRepository = userRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        [ApiAuthorize]
        public async Task<JsonResult> UserInfo()
        {
            var stopWatch = Stopwatch.StartNew();
            
            var userName = System.Web.HttpContext.Current.User.Identity.Name;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var userInfo = await _userRepository.GetUser(userName);
                var user = Json(userInfo, JsonRequestBehavior.AllowGet);

                stopWatch.Stop();
                var ts = stopWatch.Elapsed;

                _loggingService.Error($"Auth-UserInfo-Duration-Of-Call: {ts.Milliseconds}ms");

                return user;
            }
            stopWatch.Stop();
            var ts2 = stopWatch.Elapsed;

            _loggingService.Error($"Auth-UserInfoNoUser-Duration-Of-Call: {ts2.Milliseconds}ms");

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
