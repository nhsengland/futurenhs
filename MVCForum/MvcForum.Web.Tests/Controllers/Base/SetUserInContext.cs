using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MvcForum.Web.Tests.Controllers.Base
{
    public class SetUserInContext
    {
        public static void SetContext(string username)
        {
            HttpContext.Current = new HttpContext(
            new HttpRequest("", "http://tempuri.org ", ""),
            new HttpResponse(new StringWriter())
            );

            // User is logged in
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity(username),
                new string[0]
                );
        }
    }
}
