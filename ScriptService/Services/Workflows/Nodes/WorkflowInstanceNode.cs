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

        /// <summary>
        /// creates a new <see cref="WorkflowInstanceNode"/>
        /// </summary>
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for workflow call</param>
        /// <param name="scriptcompiler">compiler used to build workflow arguments</param>
        public WorkflowInstanceNode(Guid nodeid, string name, CallWorkableParameters parameters, IScriptCompiler scriptcompiler)
        : base(nodeid, name) {
            this.parameters = parameters;
            Arguments = parameters.Arguments.BuildArguments(scriptcompiler);
        }

        /// <summary>
        /// arguments for workflow call
        /// </summary>
        IDictionary<string, IScript> Arguments { get; }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            WorkflowInstance instance = await state.GetWorkflow(parameters.Name);
            object result = await state.WorkflowExecutor.Execute(instance, state.Logger, await Arguments.EvaluateArguments(state.Variables, token), token);
            if (result is SuspendState suspend)
                result = new SuspendState(this, state.Variables, suspend);

            return result;
        }
    }
}