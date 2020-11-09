using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to execute a workflow
    /// </summary>
    public class WorkflowInstanceNode : InstanceNode {
        readonly CallWorkableParameters parameters;
        readonly Func<string, Task<WorkflowInstance>> instanceprovider;
        readonly Func<WorkflowInstance, WorkableLogger, IDictionary<string, object>, CancellationToken, Task<object>> executor;

        /// <summary>
        /// creates a new <see cref="WorkflowInstanceNode"/>
        /// </summary>
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for workflow call</param>
        /// <param name="instanceprovider">used to get workflow instance from name</param>
        /// <param name="executor">executes workflows</param>
        /// <param name="scriptcompiler">compiler used to build workflow arguments</param>
        public WorkflowInstanceNode(Guid nodeid, string name, CallWorkableParameters parameters, Func<string, Task<WorkflowInstance>> instanceprovider, Func<WorkflowInstance, WorkableLogger, IDictionary<string, object>, CancellationToken, Task<object>> executor, IScriptCompiler scriptcompiler)
        : base(nodeid, name) {
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
        public override async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            WorkflowInstance instance = await instanceprovider(parameters.Name);
            object result=await executor(instance, state.Logger, await Arguments.EvaluateArguments(state.Variables, token), token);
            if (result is SuspendState suspend)
                result = new SuspendState(this, state.Variables, suspend);

            return result;
        }
    }
}