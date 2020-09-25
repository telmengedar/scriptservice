using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.JavaScript {
    /// <summary>
    /// executes workflows from scripts
    /// </summary>
    public class WorkableExecutor : IWorkableExecutor {
        readonly WorkableLogger logger;
        readonly IWorkflowService workflowservice;
        readonly IWorkflowCompiler compiler;
        readonly IWorkflowExecutionService executor;
        readonly string name;
        readonly int? revision;

        /// <summary>
        /// creates a new <see cref="WorkableExecutor"/>
        /// </summary>
        /// <param name="logger">used for logging in workflow</param>
        /// <param name="workflowservice">access to workflow data</param>
        /// <param name="compiler">compiled workflows</param>
        /// <param name="executor">executes compiled workflows</param>
        /// <param name="name">name of workflow to execute</param>
        /// <param name="revision">workflow revision (optional)</param>
        public WorkableExecutor(WorkableLogger logger, IWorkflowService workflowservice, IWorkflowCompiler compiler, IWorkflowExecutionService executor, string name, int? revision) {
            this.logger = logger;
            this.workflowservice = workflowservice;
            this.compiler = compiler;
            this.executor = executor;
            this.name = name;
            this.revision = revision;
        }

        /// <inheritdoc />
        public object Execute(IDictionary<string, object> arguments) {
            return Task.Run(async () => {
                WorkflowDetails workflow = await workflowservice.GetWorkflow(name, revision);
                WorkflowInstance instance = await compiler.BuildWorkflow(workflow, arguments);
                return await executor.Execute(instance, logger, CancellationToken.None);
            }).GetAwaiter().GetResult();
        }
    }
}