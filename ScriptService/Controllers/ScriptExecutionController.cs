using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Services;

namespace ScriptService.Controllers {

    /// <summary>
    /// provides endpoints for script execution
    /// </summary>
    [Route("api/v1/scripts/tasks")]
    [ApiController]
    public class ScriptExecutionController : ControllerBase {
        readonly ILogger<ScriptExecutionController> logger;
        readonly IScriptExecutionService executionservice;

        /// <summary>
        /// creates a new <see cref="ScriptExecutionController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="executionservice">access to script executor</param>
        public ScriptExecutionController(ILogger<ScriptExecutionController> logger, IScriptExecutionService executionservice) {
            this.executionservice = executionservice;
            this.logger = logger;
        }

        /// <summary>
        /// executes a script
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WorkableTask> Execute([FromBody]ExecuteScriptParameters parameters) {
            logger.LogInformation($"Executing {parameters.Id?.ToString() ?? parameters.Name} with parameters '{string.Join(";", parameters.Parameters?.Select(p => $"{p.Key}={p.Value}")??new string[0])}'");
            if (parameters.Id.HasValue) {
                if (!string.IsNullOrEmpty(parameters.Name))
                    throw new ArgumentException("Either id or name has to be set, not both");
                return await executionservice.Execute(parameters.Id.Value, parameters.Revision, parameters.Parameters, parameters.Wait);
            }

            if (!string.IsNullOrEmpty(parameters.Name))
                return await executionservice.Execute(parameters.Name, parameters.Revision, parameters.Parameters, parameters.Wait);

            if (parameters.Code != null)
                return await executionservice.Execute(parameters.Code, parameters.Parameters, parameters.Wait);
            throw new ArgumentException("Script id/name or scoped code to execute is required");
        }
    }
}
