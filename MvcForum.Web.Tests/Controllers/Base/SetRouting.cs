using Moq;
using MvcForum.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcForum.Web.Tests.Controllers.Base
{
    public static class SetRouting<T> where T : Controller
    {
        public static void SetupController(T controller, string routeName, string routeUrl, string routeController, string routeAction)
        {
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();

            var requestContext = new RequestContext(context.Object, new RouteData());
            var routes = new RouteCollection();

            routes.MapRoute(
                routeName,            // Route name
                routeUrl,               // URL with parameters
                new { controller = routeController, action = routeAction, slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            controller.Url = new UrlHelper(requestContext, routes);

            context.SetupGet(x => x.Request).Returns(request.Object); //Set up request base for httpcontext

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller); //set controller context
        }
    }
}
