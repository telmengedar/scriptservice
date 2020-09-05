using System.Threading.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// executes a workflow by id
    /// </summary>
    public class WorkflowIdMethod : WorkflowMethod {
        readonly long workflowid;
        readonly int? revision;
        readonly IWorkflowService workflowservice;

        /// <summary>
        /// creates a new <see cref="ScriptIdMethod"/>
        /// </summary>
        /// <param name="workflowid">id of workflow</param>
        /// <param name="revision">revision of workflow to execute</param>
        /// <param name="workflowservice">access to workflow data</param>
        /// <param name="executor">executor used to execute workflow</param>
        /// <param name="compiler">compiles workflow data for execution</param>
        public WorkflowIdMethod(long workflowid, int? revision, IWorkflowService workflowservice, IWorkflowExecutionService executor, IWorkflowCompiler compiler) 
        : base(executor, compiler) {
            this.workflowid = workflowid;
            this.revision = revision;
            this.workflowservice = workflowservice;
        }

        /// <inheritdoc />
        protected override Task<WorkflowDetails> LoadWorkflow() {
            return workflowservice.GetWorkflow(workflowid, revision);
        }
    }
}