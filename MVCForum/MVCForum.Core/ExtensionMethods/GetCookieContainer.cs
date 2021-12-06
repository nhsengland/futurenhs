using System;
using System.Net;

namespace MvcForum.Core.ExtensionMethods
{
    public static class CookieContainerExtensions
    {
        public static CookieContainer GetCookieContainer(this System.Web.HttpRequestBase sourceHttpRequest,string applicationGatewayFqdn)
        {
            var sourceCookies = sourceHttpRequest.Cookies;
            if (sourceCookies.Count == 0)
                return null;
            else
            {
                var domain = new Uri(applicationGatewayFqdn).Host;
                var cookieContainer = new CookieContainer();
                for (var i = 0; i < sourceCookies.Count; i++)
                {
                    var cSource = sourceCookies[i];
                    if (cSource != null)
                    {
                        var cookieTarget = new Cookie()
                        {
                            Domain = domain,
                            Name = cSource.Name,
                            Path = cSource.Path,
                            Secure = cSource.Secure,
                            Value = cSource.Value
                        };
                        cookieContainer.Add(cookieTarget);
                    }
                }

                return cookieContainer;
            }
        }
    }
}
