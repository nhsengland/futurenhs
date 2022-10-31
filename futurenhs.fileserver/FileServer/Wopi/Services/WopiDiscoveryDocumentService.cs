using System.Diagnostics;
using System.Xml.Linq;
using FileServer.Wopi.Interfaces;
using FutureNHS.WOPIHost.Configuration;
using Microsoft.Extensions.Options;

namespace FileServer.Wopi.Services
{
    public sealed class WopiDiscoveryDocumentService : IWopiDiscoveryDocumentService
    {
        private readonly ILogger<WopiDiscoveryDocumentService> _logger;
        private readonly WopiConfiguration _wopiConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        public WopiDiscoveryDocumentService(IOptionsSnapshot<WopiConfiguration> wopiConfiguration, ILogger<WopiDiscoveryDocumentService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _wopiConfiguration = wopiConfiguration?.Value ?? throw new ArgumentNullException(nameof(wopiConfiguration));

            var clientDiscoveryDocumentUrl = _wopiConfiguration.ClientDiscoveryDocumentUrl;

            if (clientDiscoveryDocumentUrl is null) throw new ApplicationException($"The {nameof(WopiConfiguration.ClientDiscoveryDocumentUrl)} is null");

            if (!clientDiscoveryDocumentUrl.IsAbsoluteUri) throw new ApplicationException($"The {nameof(WopiConfiguration.ClientDiscoveryDocumentUrl)} is not an absolute URI = {clientDiscoveryDocumentUrl}");
        }

        public async Task<IWopiDiscoveryDocument> GetAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var clientDiscoveryDocumentUrl = _wopiConfiguration.ClientDiscoveryDocumentUrl;

            Debug.Assert(clientDiscoveryDocumentUrl is not null);

            var httpClient = _httpClientFactory.CreateClient("wopi-discovery-document");

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, clientDiscoveryDocumentUrl);

            var xmlMediaTypes = new[] { "application/xml", "text/xml" };

            var accepts = xmlMediaTypes.Aggregate(string.Empty, (acc, n) => string.Concat(acc, n, ", "))[0..^2];

            httpRequestMessage.Headers.Add("Accept", accepts);

            try
            {               
                using var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

                if (!response.IsSuccessStatusCode) return WopiDiscoveryDocument.Empty;

                var httpContent = response.Content;

                if (httpContent is null) return WopiDiscoveryDocument.Empty;

                using var strm = await httpContent.ReadAsStreamAsync(cancellationToken);

                var xml = await XDocument.LoadAsync(strm, LoadOptions.None, cancellationToken);

                if (WopiDiscoveryDocument.IsXmlDocumentSupported(xml)) return new WopiDiscoveryDocument(clientDiscoveryDocumentUrl, xml, _logger);
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Failed to connect to the WOPI Client to download the discovery document");
            }

            return WopiDiscoveryDocument.Empty;
        }
    }
}
