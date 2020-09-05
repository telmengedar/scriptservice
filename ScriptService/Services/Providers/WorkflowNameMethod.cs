using System.Threading.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// method which executes a script by name
    /// </summary>
    public class WorkflowNameMethod : WorkflowMethod {
        readonly string workflowname;
        readonly int? revision;
        readonly IWorkflowService workflowservice;

        /// <summary>
        /// creates a new <see cref="ScriptIdMethod"/>
        /// </summary>
        /// <param name="workflowname">name of workflow</param>
        /// <param name="revision">revision of workflow to execute</param>
        /// <param name="workflowservice">access to workflow data</param>
        /// <param name="executor">executor used to execute workflow</param>
        /// <param name="compiler">compiles workflow data for execution</param>
        public WorkflowNameMethod(string workflowname, int? revision, IWorkflowService workflowservice, IWorkflowExecutionService executor, IWorkflowCompiler compiler) 
        : base(executor, compiler) {
            this.workflowname = workflowname;
            this.revision = revision;
            this.workflowservice = workflowservice;
        }

        /// <inheritdoc />
        protected override Task<WorkflowDetails> LoadWorkflow() {
            return workflowservice.GetWorkflow(workflowname, revision);
        }
    }
}