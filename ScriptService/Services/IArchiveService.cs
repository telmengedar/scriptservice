using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;

namespace ScriptService.Services {

    /// <summary>
    /// archives object revisions
    /// </summary>
    public interface IArchiveService {

        /// <summary>
        /// archives data of an object
        /// </summary>
        /// <param name="type">type of object to archive</param>
        /// <param name="id">id of object to archive</param>
        /// <param name="revision">object revision to archive</param>
        /// <param name="objectdata">data of object to archive</param>
        Task ArchiveObject<T>(string type, long id, int revision, T objectdata);

        /// <summary>
        /// get an archived object
        /// </summary>
        /// <param name="type">type of object to retrieve</param>
        /// <param name="id">id of object to retrieve</param>
        /// <param name="revision">object revision to retrieve</param>
        /// <returns>archived object data</returns>
        Task<T> GetArchivedObject<T>(string type, long id, int revision);

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