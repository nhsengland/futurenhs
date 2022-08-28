using FutureNHS.WOPIHost.Exceptions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace FutureNHS.WOPIHost
{
    public interface IWopiDiscoveryDocument : IWopiProofKeysProvider
    {
        bool IsTainted { get; set; } 

        Uri? GetEndpointForFileExtension(string fileExtension, string fileAction, Uri wopiHostFileEndpointUrl);
    }    

    internal sealed class WopiDiscoveryDocument : IWopiDiscoveryDocument
    {
        internal static readonly WopiDiscoveryDocument Empty = new();

        private readonly Uri? _sourceEndpoint;
        private readonly XDocument? _xml;
        private readonly ILogger? _logger;

        private readonly string? _publicKeyCspBlob;
        private readonly string? _oldPublicKeyCspBlob;

        private WopiDiscoveryDocument() { }

        internal WopiDiscoveryDocument(Uri sourceEndpoint, XDocument xml, ILogger? logger = default) 
        { 
            _sourceEndpoint = sourceEndpoint ?? throw new ArgumentNullException(nameof(sourceEndpoint));
            _xml = xml                       ?? throw new ArgumentNullException(nameof(xml));

            _logger = logger;

            // Extract the proof keys from the discovery document, noting there may be two to consider in the event of a key
            // rotation

            var root = _xml.Element(XName.Get("wopi-discovery"));

            if (root is null) throw new ArgumentException("The wopi discovery document xml is expected to have a root element called wopi-discovery.  The supplied xml is not considered well formed.", nameof(xml));

            var proofKey = root.Element(XName.Get("proof-key"));

            if (proofKey is null) throw new ArgumentException("The root element 'wopi-discovery' is expected to have an immediate child named 'proof-key'.  The supplied xml is not considered well formed.", nameof(xml));

            _publicKeyCspBlob = proofKey.Attribute(XName.Get("value"))?.Value;
            _oldPublicKeyCspBlob = proofKey.Attribute(XName.Get("oldvalue"))?.Value;

            if (string.IsNullOrWhiteSpace(_publicKeyCspBlob)) throw new ArgumentException("The value attribute of the proof-key element in the xml is either null or empty.  The supplied xml is not considered well formed.", nameof(xml));
            if (string.IsNullOrWhiteSpace(_oldPublicKeyCspBlob)) throw new ArgumentException("The oldValue attribute of the proof-key element in the xml is either null or empty.  The supplied xml is not considered well formed.", nameof(xml));
        }

        string? IWopiProofKeysProvider.PublicKeyCspBlob => _publicKeyCspBlob;
        string? IWopiProofKeysProvider.OldPublicKeyCspBlob => _oldPublicKeyCspBlob;

        /// <summary>
        /// Responsible for validating that the schema of the discovery document returned by the WOPI client can 
        /// be successfully navigated by this host application
        /// </summary>
        /// <param name="xml">The xml document returned to us from the WOPI client</param>
        /// <returns>true if the document looks good, else false for an invalid document</returns>
        /// <remarks>
        /// TODO - Need to use an XML schema document to fully validate it
        /// </remarks>
        internal static bool IsXmlDocumentSupported(XDocument xml)
        {
            if (xml is null) return false;

            var rootElement = xml.Element(XName.Get("wopi-discovery"));

            if (rootElement is null) return false;

            // TODO - Ensure there is a proof key section in the document

            return true;
        }

        /// <summary>
        /// Whether this document has identified itself as out-of-date and thus in need of being refetched from the 
        /// source WOPI client.  Only accurate if <see cref="IsEmpty"/> = false.
        /// </summary>
        bool IWopiDiscoveryDocument.IsTainted { get;  set; }

        /// <summary>
        /// Whether this instance of <see cref="WopiDiscoveryDocument"/> represent an 'Empty' container state (ie not a fully formed
        /// document) or not
        /// </summary>
        public bool IsEmpty => ReferenceEquals(this, Empty);

        Uri? IWopiDiscoveryDocument.GetEndpointForFileExtension(string fileExtension, string fileAction, Uri wopiHostFileEndpointUrl)
        {
            // https://wopi.readthedocs.io/en/latest/discovery.html

            const Uri? UNABLE_TO_LOCATE_ENDPOINT_FOR_FILE = default;

            if (IsEmpty) throw new WopiDiscoveryDocumentEmptyException();

            Debug.Assert(_xml is not null);

            if (string.IsNullOrWhiteSpace(fileExtension)) return default;
            if (string.IsNullOrWhiteSpace(fileAction)) return default;

            if (wopiHostFileEndpointUrl is null) return default;
            if (!wopiHostFileEndpointUrl.IsAbsoluteUri) return default;

            if (fileExtension.StartsWith('.')) fileExtension = fileExtension[1..];

            var contentTypeProvider = new FileExtensionContentTypeProvider();

            _ = contentTypeProvider.TryGetContentType("." + fileExtension, out var fileContentType);

            var rootElement = _xml.Element(XName.Get("wopi-discovery"));

            Debug.Assert(rootElement is not null);

            var netZoneElement = rootElement.Element(XName.Get("net-zone"));

            if (netZoneElement is null) return UNABLE_TO_LOCATE_ENDPOINT_FOR_FILE;

            foreach (var appElement in netZoneElement.Elements("app"))
            {
                var appName = appElement.Attribute("name")?.Value;

                if (!string.IsNullOrWhiteSpace(appName))
                {
                    foreach (var actionElement in appElement.Elements("action"))
                    {
                        if (!string.Equals(appName, fileContentType, StringComparison.OrdinalIgnoreCase))
                        {
                            var ext = actionElement.Attribute("ext")?.Value;

                            if (!string.Equals(ext, fileExtension, StringComparison.OrdinalIgnoreCase)) continue;

                            var name = actionElement.Attribute("name")?.Value; // https://wopi.readthedocs.io/en/latest/discovery.html#wopi-actions

                            if (!string.Equals(name, fileAction, StringComparison.OrdinalIgnoreCase)) continue;
                        }

                        var urlSrc = actionElement.Attribute("urlsrc")?.Value;

                        if (string.IsNullOrWhiteSpace(urlSrc)) continue;

                        urlSrc = TransformActionUrlSrcAttribute(urlSrc, wopiHostFileEndpointUrl);

                        return new Uri(string.Concat(urlSrc, "WOPISrc=", wopiHostFileEndpointUrl.AbsoluteUri), UriKind.Absolute);
                    }
                }
            }

            return UNABLE_TO_LOCATE_ENDPOINT_FOR_FILE;
        }

        /// <summary>
        /// Responsible for assuring the url attribute of an action element in the discovery document is correctly formed.
        /// This process mainly involves the replacement of placeholder values (that could not be determined by the WOPI client)
        /// with the real one that we are responsible for maintaining
        /// </summary>
        /// <param name="urlSrc"></param>
        /// <param name="wopiHostFileEndpointUrl"></param>
        /// <returns></returns>
#if DEBUG
        internal static // for local testing of private method
#else
        private 
#endif
        string TransformActionUrlSrcAttribute(string urlSrc, Uri wopiHostFileEndpointUrl)
        {
            // https://wopi.readthedocs.io/en/latest/discovery.html#transforming-the-urlsrc-parameter

            Debug.Assert(!string.IsNullOrWhiteSpace(urlSrc));
            Debug.Assert(wopiHostFileEndpointUrl is not null && wopiHostFileEndpointUrl.IsAbsoluteUri);

            // If the urlSrc contains a placeholder for the wopiSrc then we must replace with the correct value

            while (urlSrc.Contains("<WOPI_SRC=PLACEHOLDER_VALUE"))
            {
                urlSrc = urlSrc.Replace("<WOPI_SRC=PLACEHOLDER_VALUE[&]>", $"WOPI_SRC={wopiHostFileEndpointUrl.AbsoluteUri}&", StringComparison.Ordinal);
                urlSrc = urlSrc.Replace("<WOPI_SRC=PLACEHOLDER_VALUE>",    $"WOPI_SRC={wopiHostFileEndpointUrl.AbsoluteUri}",  StringComparison.Ordinal);
            }

            // MANDATORY - At this point, we have replaced all the placeholder parameters that we know about, therefore the 
            // remaining ones need to be removed so the WOPI client can use their default value

            int n;

            while (0 <= (n = urlSrc.IndexOf("<")))
            {
                var i = urlSrc.IndexOf("=PLACEHOLDER_VALUE[&]>", n, StringComparison.Ordinal);

                if (0 >= i)
                {
                    i = urlSrc.IndexOf("=PLACEHOLDER_VALUE>", n, StringComparison.Ordinal);

                    urlSrc = string.Concat(urlSrc.AsSpan(0, n), urlSrc.AsSpan(i + "=PLACEHOLDER_VALUE>".Length));
                }
                else if (0 == n)
                {
                    urlSrc = urlSrc[.."=PLACEHOLDER_VALUE[&]>".Length];
                }
                else
                { 
                    urlSrc = string.Concat(urlSrc.AsSpan(0, n), urlSrc.AsSpan(i + "=PLACEHOLDER_VALUE[&]>".Length));
                }
            }

            // If the string ends with an errant & character, remove it
            // Important to note that the string must end with a ? character for things to work correctly when building the 
            // loleaflet endpoint (undocumented feature!)

            if (urlSrc.EndsWith('&')) urlSrc = urlSrc[0..^1];

            return urlSrc;
        }
    }
}
