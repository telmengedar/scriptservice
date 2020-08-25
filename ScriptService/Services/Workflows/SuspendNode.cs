using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows.Nodes;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// node which suspends execution of workflow
    /// </summary>
    public class SuspendNode : InstanceNode {

        /// <summary>
        /// creates a new <see cref="SuspendNode"/>
        /// </summary>
        /// <param name="nodeName">name of node</param>
        /// <param name="parameters">parameters for operation</param>
        public SuspendNode(string nodeName, SuspendParameters parameters) : base(nodeName) {
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for operation
        /// </summary>
        public SuspendParameters Parameters { get; }

        /// <inheritdoc />
        public override Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            if (!string.IsNullOrEmpty(Parameters.Variable))
                state[Parameters.Variable] = null;
            SuspendState suspendstate = new SuspendState(this, variables, state);
            return Task.FromResult((object)suspendstate);
        }
    }
}