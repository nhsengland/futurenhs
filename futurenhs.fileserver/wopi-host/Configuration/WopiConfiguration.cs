using System;

namespace FutureNHS.WOPIHost.Configuration
{
    public sealed class WopiConfiguration
    {
        /// <summary>
        /// This is the absolute url for where the discovery document produced by the WOPI client is accessed from
        /// </summary>
        /// <example>
        /// http://host.docker.internal:44355/wopi/files/
        /// http://127.0.0.1:9981/hosting/discovery
        /// http://127.0.0.1:9981/gateway/wopi/client/hosting/discovery
        /// https://futurenhs.cds.co.uk/gateway/wopi/host/files/
        /// </example>

        public Uri? ClientDiscoveryDocumentUrl { get; set; }

        /// <summary>
        /// This is the absolute url for the root endpoint from which the WOPI host (the file server) implements WOPI file related
        /// protocol
        /// </summary>
        /// <example>
        /// https://futurenhs.cds.co.uk/gateway/wopi/client/hosting/discovery
        /// </example>

        public Uri? HostFilesUrl { get; set; }
    }
}
