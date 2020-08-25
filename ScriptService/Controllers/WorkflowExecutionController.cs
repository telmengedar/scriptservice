using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services;

namespace ScriptService.Controllers {

    /// <summary>
    /// provides endpoints to execute workflows
    /// </summary>
    [Route("api/v1/workflows/tasks")]
    [ApiController]
    public class WorkflowExecutionController : ControllerBase {
        readonly ILogger<WorkflowExecutionController> logger;
        readonly IWorkflowExecutionService executionservice;

        /// <summary>
        /// creates a new <see cref="WorkflowExecutionController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="executionservice">access to workflow execution service</param>
        public WorkflowExecutionController(ILogger<WorkflowExecutionController> logger, IWorkflowExecutionService executionservice) {
            this.logger = logger;
            this.executionservice = executionservice;
        }

        /// <summary>
        /// executes a script
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WorkableTask> Execute([FromBody] ExecuteWorkflowParameters parameters) {
            logger.LogInformation($"Executing {parameters.Id?.ToString() ?? parameters.Name} with parameters '{string.Join(";", parameters.Parameters?.Select(p => $"{p.Key}={p.Value}") ?? new string[0])}'");
            if(parameters.Id.HasValue) {
                if(!string.IsNullOrEmpty(parameters.Name))
                    throw new ArgumentException("Either id or name has to be set, not both");
                return await executionservice.Execute(parameters.Id.Value, parameters.Parameters, parameters.Wait);
            }

            if(!string.IsNullOrEmpty(parameters.Name))
                return await executionservice.Execute(parameters.Name, parameters.Parameters, parameters.Wait);

            if (parameters.Workflow != null)
                return await executionservice.Execute(parameters.Workflow, parameters.Parameters, parameters.Wait);

            throw new ArgumentException("Workflow id or name is required");
        }

        /// <summary>
        /// executes a script
        /// </summary>
        /// <param name="taskid">id of task to continue</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPut("{taskid}")]
        public Task<WorkableTask> Continue(Guid taskid, [FromBody] ContinueWorkflowBody parameters) {
            logger.LogInformation($"Continuing workflow task '{taskid}' with parameters '{string.Join(";", parameters.Parameters?.Select(p => $"{p.Key}={p.Value}") ?? new string[0])}'");
            return executionservice.Continue(taskid, parameters.Parameters, parameters.Wait);
        }

    }
}
