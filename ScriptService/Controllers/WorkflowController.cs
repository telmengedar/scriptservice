﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto.Patches;
using ScriptService.Dto.Workflows;
using ScriptService.Services;

namespace ScriptService.Controllers {

    /// <summary>
    /// provides endpoints for workflow data
    /// </summary>
    [Route("api/v1/workflows")]
    [ApiController]
    public class WorkflowController : ControllerBase {
        readonly ILogger<WorkflowController> logger;
        readonly IWorkflowService workflowservice;

        /// <summary>
        /// creates a new <see cref="WorkflowController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="workflowservice">access to workflow data</param>
        public WorkflowController(ILogger<WorkflowController> logger, IWorkflowService workflowservice) {
            this.logger = logger;
            this.workflowservice = workflowservice;
        }

        /// <summary>
        /// creates a new <see cref="Workflow"/>
        /// </summary>
        /// <param name="data">data of workflow to create</param>
        /// <returns>workflowid of created workflow</returns>
        [HttpPost]
        public async Task<WorkflowDetails> CreateWorkflow([FromBody] WorkflowStructure data) {
            logger.LogInformation("Creating new workflow");
            return await workflowservice.GetWorkflow(await workflowservice.CreateWorkflow(data));
        }

        /// <summary>
        /// updates data of a workflow using a complete structure
        /// </summary>
        /// <param name="workflowid">id of workflow to update</param>
        /// <param name="data">workflow data</param>
        [HttpPut("{workflowid}")]
        public async Task<WorkflowDetails> UpdateWorkflow(long workflowid, [FromBody]WorkflowStructure data) {
            logger.LogInformation($"Updating workflow '{workflowid}'");
            await workflowservice.UpdateWorkflow(workflowid, data);
            return await workflowservice.GetWorkflow(workflowid);
        }

        /// <summary>
        /// get a workflow from backend
        /// </summary>
        /// <param name="workflowid">workflowid of workflow to get</param>
        /// <returns>full workflow information</returns>
        [HttpGet("{workflowid}")]
        public Task<WorkflowDetails> GetWorkflow(long workflowid) {
            return workflowservice.GetWorkflow(workflowid);
        }

        /// <summary>
        /// lists workflows using a filter
        /// </summary>
        /// <param name="filter">filter to use when listing (optional)</param>
        /// <returns>a result page of matching workflows</returns>
        [HttpGet]
        public Task<Page<Workflow>> ListWorkflows([FromQuery]ListFilter filter = null) {
            return workflowservice.ListWorkflows(filter);
        }

        /// <summary>
        /// patches data of a workflow
        /// </summary>
        /// <param name="workflowid">id of workflow to patch</param>
        /// <param name="patches">patches to apply</param>
        [HttpPatch("{workflowid}")]
        public async Task<WorkflowDetails> PatchWorkflow(long workflowid, [FromBody]PatchOperation[] patches) {
            logger.LogInformation($"Patching workflow {workflowid}");
            await workflowservice.PatchWorkflow(workflowid, patches);
            return await workflowservice.GetWorkflow(workflowid);
        }

        /// <summary>
        /// deletes a workflow
        /// </summary>
        /// <param name="workflowid">id of workflow to delete</param>
        [HttpDelete("{workflowid}")]
        public Task DeleteWorkflow(long workflowid) {
            logger.LogInformation($"Deleting workflow {workflowid}");
            return workflowservice.DeleteWorkflow(workflowid);
        }

    }
}
