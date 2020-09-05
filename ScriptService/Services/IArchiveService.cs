using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.Database.Clients;

namespace ScriptService.Services {

    /// <summary>
    /// archives object revisions
    /// </summary>
    public interface IArchiveService {
        /// <summary>
        /// archives data of an object
        /// </summary>
        /// <param name="transaction">transaction to use</param>
        /// <param name="id">id of object to archive</param>
        /// <param name="revision">object revision to archive</param>
        /// <param name="objectdata">data of object to archive</param>
        /// <param name="objectname">name of object type to store (optional)</param>
        Task ArchiveObject<T>(Transaction transaction, long id, int revision, T objectdata, string objectname=null);

        /// <summary>
        /// get an archived object
        /// </summary>
        /// <param name="id">id of object to retrieve</param>
        /// <param name="revision">object revision to retrieve</param>
        /// <param name="objectname">name of object type to retrieve (optional)</param>
        /// <returns>archived object data</returns>
        Task<T> GetArchivedObject<T>(long id, int revision, string objectname=null);

        /// <summary>
        /// lists revisions numbers archived for an object
        /// </summary>
        /// <param name="type">type of archived object</param>
        /// <param name="id">id of archived object</param>
        /// <param name="filter">paging filter to apply</param>
        /// <returns>a page of available object revisions</returns>
        Task<Page<int>> ListRevisions(string type, long id, ListFilter filter=null);
    }
}