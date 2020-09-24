using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to execute a workflow
    /// </summary>
    public class WorkflowInstanceNode : InstanceNode {
        readonly CallWorkableParameters parameters;
        readonly Func<string, IDictionary<string, object>, Task<WorkflowInstance>> instanceprovider;
        readonly Func<WorkflowInstance, WorkableLogger, CancellationToken, Task<object>> executor;

        /// <summary>
        /// creates a new <see cref="WorkflowInstanceNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for workflow call</param>
        /// <param name="instanceprovider">used to get workflow instance from name</param>
        /// <param name="executor">executes workflows</param>
        /// <param name="scriptcompiler">compiler used to build workflow arguments</param>
        public WorkflowInstanceNode(string name, CallWorkableParameters parameters, Func<string, IDictionary<string, object>, Task<WorkflowInstance>> instanceprovider, Func<WorkflowInstance, WorkableLogger, CancellationToken, Task<object>> executor, IScriptCompiler scriptcompiler)
        : base(name) {
            this.parameters = parameters;
            this.instanceprovider = instanceprovider;
            this.executor = executor;
            Arguments = parameters.Arguments.BuildArguments(scriptcompiler);
        }

        /// <summary>
        /// arguments for workflow call
        /// </summary>
        IDictionary<string, IScript> Arguments { get; }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            WorkflowInstance instance = await instanceprovider(parameters.Name, await Arguments.EvaluateArguments(state, token));
            object result=await executor(instance, logger, token);
            if (result is SuspendState suspend)
                result = new SuspendState(this, variables, state, suspend);

            return result;
        }
    }
}