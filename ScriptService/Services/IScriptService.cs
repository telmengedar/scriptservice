using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto;
using ScriptService.Dto.Patches;

namespace ScriptService.Services {

    /// <summary>
    /// script data management
    /// </summary>
    public interface IScriptService {

        /// <summary>
        /// creates a new script
        /// </summary>
        /// <param name="script">data for script to create</param>
        /// <returns>id of created script</returns>
        Task<long> CreateScript(ScriptData script);

        /// <summary>
        /// get a script by id
        /// </summary>
        /// <param name="scriptid">id of script to get</param>
        /// <returns>script with the specified id</returns>
        Task<Script> GetScript(long scriptid);

        /// <summary>
        /// get a script by scriptname
        /// </summary>
        /// <param name="scriptname">name of script</param>
        /// <returns>script with the specified name</returns>
        Task<Script> GetScript(string scriptname);

        /// <summary>
        /// lists scripts matching a criteria
        /// </summary>
        /// <param name="filter">filter for scripts to match</param>
        /// <returns>a page of scripts which match the filter</returns>
        Task<Page<Script>> ListScripts(ListFilter filter=null);

        /// <summary>
        /// patches properties of a script
        /// </summary>
        /// <param name="scriptid">id of script to patch</param>
        /// <param name="patches">patches to apply</param>
        Task PatchScript(long scriptid, PatchOperation[] patches);

        /// <summary>
        /// deletes an existing script
        /// </summary>
        /// <param name="scriptid">id of script to delete</param>
        Task DeleteScript(long scriptid);
    }
}