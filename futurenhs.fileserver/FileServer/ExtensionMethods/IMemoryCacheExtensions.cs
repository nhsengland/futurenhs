using Microsoft.Extensions.Caching.Memory;
using System;
using FileServer.Wopi;

namespace FutureNHS.WOPIHost
{
    public static partial class ExtensionMethods
    {
        internal const string WOPI_DISCOVERY_DOCUMENT_CACHE_KEY = "wopi.discovery.document";

        public static bool TrySetWopiDiscoveryDocument(this IMemoryCache memoryCache, IWopiDiscoveryDocument? wopiDiscoveryDocument)
        {
            if (memoryCache is null) return false;

            if (wopiDiscoveryDocument is null) return TryRemoveWopiDiscoveryDocument(memoryCache);

            try
            {
                memoryCache.Set(WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, wopiDiscoveryDocument, absoluteExpirationRelativeToNow: TimeSpan.FromHours(12));

                return true;
            }
            catch (ApplicationException) { }

            return memoryCache.TryGetValue(WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, out _);
        }

        public static bool TryGetWopiDiscoveryDocument(this IMemoryCache memoryCache, out IWopiDiscoveryDocument? wopiDiscoveryDocument)
        {
            wopiDiscoveryDocument = default;

            if (memoryCache is null) return false;

            return memoryCache.TryGetValue(WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, out wopiDiscoveryDocument);
        }

        static bool TryRemoveWopiDiscoveryDocument(IMemoryCache? memoryCache)
        {
            if (memoryCache is null) return false;

            try
            {
                if (!memoryCache.TryGetValue(WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, out _)) return true;

                memoryCache.Remove(WOPI_DISCOVERY_DOCUMENT_CACHE_KEY);

                return true;
            }
            catch (Exception) { }

            return !memoryCache.TryGetWopiDiscoveryDocument(out _);
        }
    }
}
