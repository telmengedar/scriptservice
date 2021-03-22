using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.Lua {
    /// <summary>
    /// executes workflows from scripts
    /// </summary>
    public class LuaWorkableExecutor : IWorkableExecutor {
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
        public LuaWorkableExecutor(WorkableLogger logger, IWorkflowService workflowservice, IWorkflowCompiler compiler, IWorkflowExecutionService executor, string name, int? revision) {
            this.logger = logger;
            this.workflowservice = workflowservice;
            this.compiler = compiler;
            this.executor = executor;
            this.name = name;
            this.revision = revision;
        }

        /// <inheritdoc />
        public object Execute(IDictionary<string, object> arguments) {
            arguments.TranslateDictionary();
            return Task.Run(async () => {
                WorkflowDetails workflow = await workflowservice.GetWorkflow(name, revision);
                WorkflowInstance instance = await compiler.BuildWorkflow(workflow);
                return await executor.Execute(instance, logger, arguments, false, CancellationToken.None);
            }).GetAwaiter().GetResult();
        }
    }
}