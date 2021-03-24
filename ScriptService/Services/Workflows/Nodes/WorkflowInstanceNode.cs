using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using ScriptService.Dto;
using ScriptService.Dto.Workflows;
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
        /// <param name="language">language used to translate arguments</param>
        public WorkflowInstanceNode(Guid nodeid, string name, CallWorkableParameters parameters, IScriptCompiler scriptcompiler, ScriptLanguage? language)
        : base(nodeid, name) {
            this.parameters = parameters;
            Arguments = parameters.Arguments.BuildArguments(scriptcompiler, language ?? ScriptLanguage.NCScript);
        }

        /// <summary>
        /// arguments for workflow call
        /// </summary>
        IDictionary<string, IScript> Arguments { get; }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            WorkflowInstance instance = await state.GetWorkflow(parameters.Name);
            WorkflowIdentifier parent = state.Workflow;
            state.Workflow = new WorkflowIdentifier(instance.Id, instance.Revision, instance.Name);
            object result = await state.WorkflowExecutor.Execute(instance, state.Logger, await Arguments.EvaluateArguments(state.Variables, token), state.Profiling, token);
            state.Workflow = parent;
            if (result is SuspendState suspend)
                result = new SuspendState(state.Workflow, this, state.Variables, state.Language, state.Profiling, suspend);

            return result;
        }
    }
}