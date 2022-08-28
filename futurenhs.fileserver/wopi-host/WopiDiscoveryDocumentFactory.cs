using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    public interface IWopiDiscoveryDocumentFactory
    {
        Task<IWopiDiscoveryDocument> CreateDocumentAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Tasked with obtaining a valid WOPI discovery document from the WOPI-Client
    /// </summary>
    /// <remarks>
    /// The discovery document contains important information with respect to what file types the client supports along 
    /// with details of the public part of a crytographic pair that the client will use to sign requests so we can be 
    /// sure they are coming from our trusted source and haven't been tampered with in transit.
    /// It is important to note that this pair of keys can be recycled and when this happens we need to refresh the 
    /// document direct from source to get the new details (the old keys stay alive for a short period)
    /// </remarks>
    public sealed class WopiDiscoveryDocumentFactory
        : IWopiDiscoveryDocumentFactory
    {
        private readonly ILogger<WopiDiscoveryDocumentFactory> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IWopiDiscoveryDocumentRepository _wopiDiscoveryDocumentRepository;

        public WopiDiscoveryDocumentFactory(IMemoryCache memoryCache, IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository, ILogger<WopiDiscoveryDocumentFactory> logger)
        {
            _logger = logger;
            
            _memoryCache = memoryCache                                          ?? throw new ArgumentNullException(nameof(memoryCache));
            _wopiDiscoveryDocumentRepository = wopiDiscoveryDocumentRepository  ?? throw new ArgumentNullException(nameof(wopiDiscoveryDocumentRepository));
        }

        async Task<IWopiDiscoveryDocument> IWopiDiscoveryDocumentFactory.CreateDocumentAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _memoryCache.TryGetWopiDiscoveryDocument(out var wopiDiscoveryDocument);

            if (wopiDiscoveryDocument is null || wopiDiscoveryDocument.IsTainted)
            {
                _logger?.LogTrace("Determined we need to try to fetch the WOPI discovery document");

                wopiDiscoveryDocument = await _wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

                _logger?.LogTrace("WOPI discovery document {RESULT}", (wopiDiscoveryDocument.IsEmpty ? "is empty" : "successfully downloaded"));

                cancellationToken.ThrowIfCancellationRequested();

                if (wopiDiscoveryDocument.IsEmpty) wopiDiscoveryDocument = default;

                _memoryCache.TrySetWopiDiscoveryDocument(wopiDiscoveryDocument);
            }

            return wopiDiscoveryDocument ?? WopiDiscoveryDocument.Empty;
        }
    }
}
