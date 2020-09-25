﻿using System;
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

        /// <summary>
        /// creates a new <see cref="WorkflowExecutionController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="executionservice">access to workflow execution service</param>
        /// <param name="compiler">compiles workflow data for execution</param>
        public WorkflowExecutionController(ILogger<WorkflowExecutionController> logger, IWorkflowExecutionService executionservice, IWorkflowCompiler compiler) {
            this.logger = logger;
            this.executionservice = executionservice;
            this.compiler = compiler;
        }

        /// <summary>
        /// executes a script
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WorkableTask> Execute([FromBody] ExecuteWorkflowParameters parameters) {
            logger.LogInformation($"Executing {parameters.Id?.ToString() ?? parameters.Name} with parameters '{string.Join(";", parameters.Parameters?.Select(p => $"{p.Key}={p.Value}") ?? new string[0])}'");

            try {
                if (parameters.Id.HasValue) {
                    if (!string.IsNullOrEmpty(parameters.Name))
                        throw new ArgumentException("Either id or name has to be set, not both");
                    return await executionservice.Execute(await compiler.BuildWorkflow(parameters.Id.Value, parameters.Revision, parameters.Parameters), parameters.Wait);
                }

                if (!string.IsNullOrEmpty(parameters.Name))
                    return await executionservice.Execute(await compiler.BuildWorkflow(parameters.Name, parameters.Revision, parameters.Parameters), parameters.Wait);

                if (parameters.Workflow != null)
                    return await executionservice.Execute(await compiler.BuildWorkflow(parameters.Workflow, parameters.Parameters), parameters.Wait);
            }
            catch (Exception e) {
                return new WorkableTask {
                    Status = TaskStatus.Failure,
                    Started = DateTime.Now,
                    Finished = DateTime.Now,
                    Log = new List<string>(new[] {
                        e.ToString()
                    }),
                    Parameters = parameters.Parameters,
                    Type = WorkableType.Workflow,
                };
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
            logger.LogInformation($"Continuing workflow task '{taskid}' with parameters '{string.Join(";", parameters.Parameters?.Select(p => $"{p.Key}={p.Value}") ?? new string[0])}'");
            return executionservice.Continue(taskid, parameters.Parameters, parameters.Wait);
        }

    }
}
