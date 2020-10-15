using System.Threading.Tasks;
using ScriptService.Dto.Workflows;

namespace ScriptService.Services {
    
    /// <summary>
    /// exports workflows in structure format
    /// </summary>
    public interface IWorkflowExportService {
        
        /// <summary>
        /// get a workflow in export format
        /// </summary>
        /// <param name="workflowid">workflowid of workflow to export</param>
        /// <param name="revision">revision of workflow to export</param>
        /// <returns>workflow structure which can get imported by another service</returns>
        Task<WorkflowStructure> ExportWorkflow(long workflowid, int? revision=null);

    }
}