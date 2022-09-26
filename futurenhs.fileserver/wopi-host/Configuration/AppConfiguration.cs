using System;

namespace FutureNHS.WOPIHost.Configuration
{
    public record AppConfiguration
    {
        public Uri? MvcForumUserInfoUrl { get; set; }
        public Uri? MvcForumHealthCheckUrl { get; set; }
    }
}
