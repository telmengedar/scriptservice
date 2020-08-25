using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto.Patches;
using ScriptService.Dto.Workflows;

namespace ScriptService.Services {

    /// <summary>
    /// service for workflow data management
    /// </summary>
    public interface IWorkflowService {

        /// <summary>
        /// creates a new <see cref="Workflow"/>
        /// </summary>
        /// <param name="data">data of workflow to create</param>
        /// <returns>workflowid of created workflow</returns>
        Task<long> CreateWorkflow(WorkflowStructure data);

        /// <summary>
        /// updates data of a workflow using a complete structure
        /// </summary>
        /// <param name="workflowid">id of workflow to update</param>
        /// <param name="data">workflow data</param>
        Task UpdateWorkflow(long workflowid, WorkflowStructure data);

        /// <summary>
        /// get a workflow from backend
        /// </summary>
        /// <param name="workflowid">workflowid of workflow to get</param>
        /// <returns>full workflow information</returns>
        Task<WorkflowDetails> GetWorkflow(long workflowid);

        /// <summary>
        /// get a workflow from backend
        /// </summary>
        /// <param name="name">name of workflow to get</param>
        /// <returns>full workflow information</returns>
        Task<WorkflowDetails> GetWorkflow(string name);

        /// <summary>
        /// lists workflows using a filter
        /// </summary>
        /// <param name="filter">filter to use when listing (optional)</param>
        /// <returns>a result page of matching workflows</returns>
        Task<Page<Workflow>> ListWorkflows(ListFilter filter = null);

        /// <summary>
        /// patches data of a workflow
        /// </summary>
        /// <param name="workflowid">id of workflow to patch</param>
        /// <param name="patches">patches to apply</param>
        Task PatchWorkflow(long workflowid, PatchOperation[] patches);

        /// <summary>
        /// deletes a workflow
        /// </summary>
        /// <param name="workflowid">id of workflow to delete</param>
        Task DeleteWorkflow(long workflowid);
    }
}