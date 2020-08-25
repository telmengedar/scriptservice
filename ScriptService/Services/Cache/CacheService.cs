using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using ScriptService.Dto.Cache;

namespace ScriptService.Services.Cache {

    /// <inheritdoc />
    public class CacheService : ICacheService {
        readonly ILogger<CacheService> logger;
        readonly ConcurrentDictionary<object, CacheEntry> cache = new ConcurrentDictionary<object, CacheEntry>();
        Timer cachetimer;
        readonly TimeSpan timeout = TimeSpan.FromMinutes(5.0);

        /// <summary>
        /// creates a new <see cref="CacheService"/>
        /// </summary>
        public CacheService(ILogger<CacheService> logger) {
            this.logger = logger;
            cachetimer = new Timer(CheckCache, null, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0));
        }

        void CheckCache(object state) {
            try {
                DateTime now = DateTime.Now;
                foreach(KeyValuePair<object, CacheEntry> entry in cache)
                    if (now - entry.Value.LastAccess > timeout) {
                        logger.LogInformation($"Cache entry for '{entry.Key}' expired. Removing entry from cache");
                        cache.TryRemove(entry.Key, out CacheEntry _);
                    }
            }
            catch (Exception e) {
                logger.LogError(e, "Error checking cache");
            }
        }

        /// <inheritdoc />
        public T GetObject<T, TId>(TId id, int revision) {
            if (cache.TryGetValue(new CacheKey<TId>(typeof(T), id, revision), out CacheEntry entry)) {
                entry.LastAccess = DateTime.Now;
                return (T) entry.Object;
            }

            return default;
        }

        /// <inheritdoc />
        public void StoreObject<T, TId>(TId id, int revision, T instance) {
            cache[new CacheKey<TId>(typeof(T), id, revision)] = new CacheEntry {
                Object = instance,
                LastAccess = DateTime.Now
            };
        }
    }
}