using System.Net;

namespace FutureNHS.Api.Extensions
{
    public static class CookieContainerExtensions
    {
        public static CookieContainer GetCookieContainer(this HttpRequest sourceHttpRequest, string applicationGatewayFqdn)
        {
            var sourceCookies = sourceHttpRequest.Cookies;
            if (sourceCookies.Count == 0)
                return null;
            else
            {
                var domain = new Uri(applicationGatewayFqdn).Host;
                var cookieContainer = new CookieContainer();
                foreach (var cSource in sourceCookies)
                {
                    var cookieTarget = new Cookie()
                    {
                        Domain = domain,
                        Name = cSource.Key,
                        Value = cSource.Value
                    };
                    cookieContainer.Add(cookieTarget);
                }

                return cookieContainer;
            }
        }
    }
}