using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows;
using ScriptService.Errors;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// base class for callable workflows
    /// </summary>
    public abstract class WorkflowMethod : IExternalMethod {
        readonly IWorkflowExecutionService executor;
        readonly IWorkflowCompiler compiler;

        /// <summary>
        /// creates a new <see cref="WorkflowMethod"/>
        /// </summary>
        /// <param name="executor">executor used to execute workflow</param>
        /// <param name="compiler">compiles workflow data for execution</param>
        protected WorkflowMethod(IWorkflowExecutionService executor, IWorkflowCompiler compiler) {
            this.executor = executor;
            this.compiler = compiler;
        }

        /// <summary>
        /// loads workflow to execute
        /// </summary>
        /// <returns>loaded workflow</returns>
        protected abstract Task<WorkflowDetails> LoadWorkflow();

        /// <inheritdoc />
        public object Invoke(IVariableProvider variables, params object[] arguments) {
            if (!(variables.GetProvider("log")?["log"] is WorkableLogger logger))
                throw new WorkflowException($"Calling a workflow as method requires an existing logger of type '{nameof(WorkableLogger)}' accessible under variable 'log'");

            if(!(arguments.FirstOrDefault() is IDictionary scriptarguments))
                throw new InvalidOperationException($"Parameters for a workflow call need to be a dictionary ('{arguments.FirstOrDefault()?.GetType()}')");

            if(!(scriptarguments is IDictionary<string, object> parameters)) {
                parameters = new Dictionary<string, object>();
                foreach(object key in scriptarguments.Keys) {
                    parameters[key.ToString() ?? string.Empty] = scriptarguments[key];
                }
            }

            return Task.Run(async () => {
                WorkflowDetails workflow = await LoadWorkflow();
                WorkflowInstance instance = await compiler.BuildWorkflow(workflow);
                return await executor.Execute(instance, logger, parameters, CancellationToken.None);
            }).GetAwaiter().GetResult();
        }
    }
}