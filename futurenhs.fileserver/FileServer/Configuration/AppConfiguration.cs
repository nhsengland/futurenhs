using System;

namespace FutureNHS.WOPIHost.Configuration
{
    public record AppConfiguration
    {
        public string UserInfoUrl { get; set; }
        public string TemplateUrlFileIdPlaceholder { get; set; }
    }
}
