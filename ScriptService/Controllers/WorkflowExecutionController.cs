using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services;
using ScriptService.Services.Workflows;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Controllers {

    /// <summary>
    /// provides endpoints to execute workflows
    /// </summary>
    [Route("api/v1/workflows/tasks")]
    [ApiController]
    public class WorkflowExecutionController : ControllerBase {
        readonly ILogger<WorkflowExecutionController> logger;
        readonly IWorkflowCompiler compiler;
        readonly IWorkflowExecutionService executionservice;
        readonly ITaskService taskservice;

        /// <summary>
        /// creates a new <see cref="WorkflowExecutionController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="executionservice">access to workflow execution service</param>
        /// <param name="compiler">compiles workflow data for execution</param>
        /// <param name="taskservice">access to task objects</param>
        public WorkflowExecutionController(ILogger<WorkflowExecutionController> logger, IWorkflowExecutionService executionservice, IWorkflowCompiler compiler, ITaskService taskservice) {
            this.logger = logger;
            this.executionservice = executionservice;
            this.compiler = compiler;
            this.taskservice = taskservice;
        }

        /// <summary>
        /// executes a script
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WorkableTask> Execute([FromBody] ExecuteWorkflowParameters parameters) {
            logger.LogInformation("Executing {workflow} with parameters '{parameters}'",
                parameters.Id?.ToString() ?? parameters.Name,
                string.Join(";", parameters.Parameters?.Select(p => $"{p.Key}={p.Value}") ?? new string[0]));

            try {
                if (parameters.Id.HasValue) {
                    if (!string.IsNullOrEmpty(parameters.Name))
                        throw new ArgumentException("Either id or name has to be set, not both");
                    return await executionservice.Execute(await compiler.BuildWorkflow(parameters.Id.Value, parameters.Revision), parameters.Parameters, parameters.Profile ?? false, parameters.Wait);
                }

                if (!string.IsNullOrEmpty(parameters.Name))
                    return await executionservice.Execute(await compiler.BuildWorkflow(parameters.Name, parameters.Revision), parameters.Parameters, parameters.Profile ?? false, parameters.Wait);

                if (parameters.Workflow != null)
                    return await executionservice.Execute(await compiler.BuildWorkflow(parameters.Workflow), parameters.Parameters, parameters.Profile ?? false, parameters.Wait);
            }
            catch (Exception e) {
                WorkableTask failtask = new WorkableTask {
                    Id = Guid.NewGuid(),
                    Status = TaskStatus.Failure,
                    Started = DateTime.Now,
                    Finished = DateTime.Now,
                    Log = new List<string>(new[] {
                        e.ToString()
                    }),
                    Parameters = parameters.Parameters,
                    Type = WorkableType.Workflow,
                };
                await taskservice.StoreTask(failtask);
                return failtask;
            }

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
            logger.LogInformation("Continuing workflow task '{taskid}' with parameters '{parameters}'",
                taskid,
                string.Join(";", parameters.Parameters?.Select(p => $"{p.Key}={p.Value}") ?? new string[0]));
            return executionservice.Continue(taskid, parameters.Parameters, parameters.Wait);
        }
    }
}
