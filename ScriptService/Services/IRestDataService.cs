using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto.Patches;

namespace ScriptService.Services {

    /// <summary>
    /// service used to store and retrieve jobs
    /// </summary>
    public interface IRestDataService<TEntity, TData, TFilter>
    where TEntity : TData 
    where TFilter : ListFilter
    {

        /// <summary>
        /// creates a new entity
        /// </summary>
        /// <param name="data">data of entity to create</param>
        /// <returns>id of created entity</returns>
        Task<long> Create(TData data);

        /// <summary>
        /// get an entity by id
        /// </summary>
        /// <param name="id">id of entity to get</param>
        /// <returns>entity with the specified id</returns>
        Task<TEntity> GetById(long id);

        /// <summary>
        /// lists a page of entities which match the specified filter
        /// </summary>
        /// <param name="filter">filter for entities to match</param>
        /// <returns>page of entities which match the filter</returns>
        Task<Page<TEntity>> List(TFilter filter = null);

        /// <summary>
        /// updates an entity with a collection of patches
        /// </summary>
        /// <param name="id">id of entity to patch</param>
        /// <param name="patches">patches to apply</param>
        Task Update(long id, PatchOperation[] patches);

        /// <summary>
        /// deletes an entity
        /// </summary>
        /// <param name="id">id of entity to delete</param>
        Task Delete(long id);
    }
}