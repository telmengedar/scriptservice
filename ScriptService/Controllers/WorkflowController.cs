using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto;
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
        readonly IWorkflowExportService exportservice;
        readonly IArchiveService archiveservice;

        /// <summary>
        /// creates a new <see cref="WorkflowController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="workflowservice">access to workflow data</param>
        /// <param name="exportservice">exports workflow data to structures</param>
        /// <param name="archiveservice">archived object data</param>
        public WorkflowController(ILogger<WorkflowController> logger, IWorkflowService workflowservice, IWorkflowExportService exportservice, IArchiveService archiveservice) {
            this.logger = logger;
            this.workflowservice = workflowservice;
            this.exportservice = exportservice;
            this.archiveservice = archiveservice;
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
            logger.LogInformation("Updating workflow '{workflowid}'", workflowid);
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
        /// get a workflow from backend
        /// </summary>
        /// <param name="workflowid">workflowid of workflow to get</param>
        /// <param name="revision">workflow revision to load</param>
        /// <returns>full workflow information</returns>
        [HttpGet("{workflowid}/{revision}")]
        public Task<WorkflowDetails> GetWorkflow(long workflowid, int revision) {
            return archiveservice.GetArchivedObject<WorkflowDetails>(workflowid, revision, ArchiveTypes.Workflow);
        }

        /// <summary>
        /// get a workflow from backend
        /// </summary>
        /// <param name="workflowid">workflowid of workflow to get</param>
        /// <returns>full workflow information</returns>
        [HttpGet("{workflowid}/export")]
        public Task<WorkflowStructure> ExportWorkflow(long workflowid) {
            return exportservice.ExportWorkflow(workflowid);
        }

        /// <summary>
        /// lists workflows using a filter
        /// </summary>
        /// <param name="filter">filter to use when listing (optional)</param>
        /// <returns>a result page of matching workflows</returns>
        [HttpGet]
        public Task<Page<Workflow>> ListWorkflows([FromQuery]WorkflowFilter filter = null) {
            return workflowservice.ListWorkflows(filter);
        }

        /// <summary>
        /// patches data of a workflow
        /// </summary>
        /// <param name="workflowid">id of workflow to patch</param>
        /// <param name="patches">patches to apply</param>
        [HttpPatch("{workflowid}")]
        public async Task<WorkflowDetails> PatchWorkflow(long workflowid, [FromBody]PatchOperation[] patches) {
            logger.LogInformation("Patching workflow {workflowid}", workflowid);
            await workflowservice.PatchWorkflow(workflowid, patches);
            return await workflowservice.GetWorkflow(workflowid);
        }

        /// <summary>
        /// deletes a workflow
        /// </summary>
        /// <param name="workflowid">id of workflow to delete</param>
        [HttpDelete("{workflowid}")]
        public Task DeleteWorkflow(long workflowid) {
            logger.LogInformation("Deleting workflow {workflowid}", workflowid);
            return workflowservice.DeleteWorkflow(workflowid);
        }

    }
}
