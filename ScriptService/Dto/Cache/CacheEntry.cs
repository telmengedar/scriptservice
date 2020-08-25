using System;

namespace ScriptService.Dto.Cache {

    /// <summary>
    /// entry for an object cache
    /// </summary>
    public class CacheEntry {

        /// <summary>
        /// object stored in cache
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        /// last time object was accessed
        /// </summary>
        public DateTime LastAccess { get; set; }
    }
}