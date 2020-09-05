using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to execute a workflow
    /// </summary>
    public class WorkflowInstanceNode : InstanceNode {
        readonly CallWorkableParameters parameters;
        readonly Func<string, Dictionary<string, object>, Task<WorkflowInstance>> instanceprovider;
        readonly Func<WorkflowInstance, WorkableLogger, CancellationToken, Task<object>> executor;
        readonly ScriptArgument[] arguments;

        /// <summary>
        /// creates a new <see cref="WorkflowInstanceNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for workflow call</param>
        /// <param name="instanceprovider">used to get workflow instance from name</param>
        /// <param name="executor">executes workflows</param>
        public WorkflowInstanceNode(string name, CallWorkableParameters parameters, Func<string, Dictionary<string, object>, Task<WorkflowInstance>> instanceprovider, Func<WorkflowInstance, WorkableLogger, CancellationToken, Task<object>> executor)
        : base(name) {
            this.parameters = parameters;
            this.instanceprovider = instanceprovider;
            this.executor = executor;
            arguments = parameters.Arguments.BuildArguments().ToArray();
        }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            WorkflowInstance instance = await instanceprovider(parameters.Name, arguments.BuildArguments(state));
            object result=await executor(instance, logger, token);
            if (result is SuspendState suspend)
                result = new SuspendState(this, variables, state, suspend);

            return result;
        }
    }
}