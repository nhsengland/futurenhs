using System;
using System.Net;
using System.Web.Mvc;

namespace MvcForum.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            else 
            {
                filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}