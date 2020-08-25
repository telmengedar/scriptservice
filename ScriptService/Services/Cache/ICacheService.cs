using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScriptService.Services.Cache {

    /// <summary>
    /// cache for objects
    /// </summary>
    public interface ICacheService {
        /// <summary>
        /// tries to get an object from cache
        /// </summary>
        /// <typeparam name="T">type of object to get</typeparam>
        /// <typeparam name="TId">type of id used to identify object</typeparam>
        /// <param name="id">id of object</param>
        /// <param name="revision">object revision</param>
        /// <returns>object if found in cache, false otherwise</returns>
        T GetObject<T, TId>(TId id, int revision);

        /// <summary>
        /// stores an object in cache
        /// </summary>
        /// <typeparam name="T">type of object to store</typeparam>
        /// <typeparam name="TId">type of id used to identify object</typeparam>
        /// <param name="id">id of object</param>
        /// <param name="revision">object revision</param>
        /// <param name="instance">object instance</param>
        void StoreObject<T, TId>(TId id, int revision, T instance);
    }
}