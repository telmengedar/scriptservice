using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto;
using ScriptService.Dto.Patches;
using ScriptService.Services;

namespace ScriptService.Controllers {

    /// <summary>
    /// provides endpoints for script code
    /// </summary>
    [Route("api/v1/scripts")]
    [ApiController]
    public class ScriptController : ControllerBase {
        readonly ILogger<ScriptController> logger;
        readonly IScriptService scriptservice;
        readonly IArchiveService archiveservice;

        /// <summary>
        /// creates a new <see cref="ScriptController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="scriptservice">access to script service</param>
        /// <param name="archiveservice">archived object data</param>
        public ScriptController(ILogger<ScriptController> logger, IScriptService scriptservice, IArchiveService archiveservice) {
            this.logger = logger;
            this.scriptservice = scriptservice;
            this.archiveservice = archiveservice;
        }

        /// <summary>
        /// creates a new script
        /// </summary>
        /// <param name="script">data for script to create</param>
        /// <returns>id of created script</returns>
        [HttpPost]
        public async Task<Script> CreateScript(ScriptData script) {
            logger.LogInformation("Creating new script {scriptname}", script.Name);
            return await scriptservice.GetScript(await scriptservice.CreateScript(script));
        }

        /// <summary>
        /// get a script by id
        /// </summary>
        /// <param name="scriptid">id of script to get</param>
        /// <returns>script with the specified id</returns>
        [HttpGet("{scriptid}")]
        public Task<Script> GetScript(long scriptid) {
            return scriptservice.GetScript(scriptid);
        }

        /// <summary>
        /// get a script by id
        /// </summary>
        /// <param name="scriptid">id of script to get</param>
        /// <param name="revision">script revision to load</param>
        /// <returns>script with the specified id</returns>
        [HttpGet("{scriptid}/{revision}")]
        public Task<Script> GetScript(long scriptid, int revision) {
            return archiveservice.GetArchivedObject<Script>(scriptid, revision);
        }

        /// <summary>
        /// lists scripts matching a criteria
        /// </summary>
        /// <param name="filter">filter for scripts to match</param>
        /// <returns>a page of scripts which match the filter</returns>
        [HttpGet]
        public Task<Page<Script>> ListScripts([FromQuery]ListFilter filter) {
            return scriptservice.ListScripts(filter);
        }

        /// <summary>
        /// patches properties of a script
        /// </summary>
        /// <param name="scriptid">id of script to patch</param>
        /// <param name="patches">patches to apply</param>
        [HttpPatch("{scriptid}")]
        public async Task<Script> PatchScript(long scriptid, [FromBody] PatchOperation[] patches) {
            logger.LogInformation("Patching script {scriptid}", scriptid);
            await scriptservice.PatchScript(scriptid, patches);
            return await scriptservice.GetScript(scriptid);
        }

        /// <summary>
        /// deletes an existing script
        /// </summary>
        /// <param name="scriptid">id of script to delete</param>
        [HttpDelete("{scriptid}")]
        public Task DeleteScript(long scriptid) {
            logger.LogInformation("Deleting script {scriptid}", scriptid);
            return scriptservice.DeleteScript(scriptid);
        }
    }
}
