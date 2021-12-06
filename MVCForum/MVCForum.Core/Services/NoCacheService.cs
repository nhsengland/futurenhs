namespace MvcForum.Core.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Web;
    using Interfaces.Services;
    using Models.Enums;

    public partial class NoCacheService : ICacheService
    {
        #region Long Cache

        private static ObjectCache Cache => MemoryCache.Default;

        private static IDictionaryEnumerator GetCacheToEnumerate()
        {
            return new Dictionary<string,string>.Enumerator();
        }

        public T Get<T>(string key)
        {
            return default(T);
        }

        /// <summary>
        ///     Cache objects for a specified amount of time
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="data">Object / Data to cache</param>
        /// <param name="minutesToCache">How many minutes to cache them for</param>
        public void Set(string key, object data, CacheTimes minutesToCache)
        {
           
        }

        public bool IsSet(string key)
        {
            return false;
        }

        public void Invalidate(string key)
        {
        }

        public void Clear()
        {
        }

        public void ClearStartsWith(string keyStartsWith)
        {
        }

        public void ClearStartsWith(List<string> keysStartsWith)
        {
        }

        #endregion

        #region Short Per Request Cache

        public T CachePerRequest<T>(string cacheKey, Func<T> getCacheItem)
        {
            return default(T);
        }

        public void SetPerRequest(string cacheKey, object objectToCache)
        {
 
        }

        #endregion
    }
}