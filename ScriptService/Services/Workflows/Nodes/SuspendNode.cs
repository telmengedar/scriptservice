using System;
using System.Threading;
using System.Threading.Tasks;
using ScriptService.Dto.Workflows.Nodes;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node which suspends execution of workflow
    /// </summary>
    public class SuspendNode : InstanceNode {
        
        /// <summary>
        /// creates a new <see cref="SuspendNode"/>
        /// </summary>
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="nodeName">name of node</param>
        /// <param name="parameters">parameters for operation</param>
        public SuspendNode(Guid nodeid, string nodeName, SuspendParameters parameters) 
            : base(nodeid, nodeName) {
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for operation
        /// </summary>
        public SuspendParameters Parameters { get; }

        /// <inheritdoc />
        public override Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            if (!string.IsNullOrEmpty(Parameters.Variable))
                state.Variables[Parameters.Variable] = null;
            SuspendState suspendstate = new SuspendState(this, state.Variables, state.Language);
            return Task.FromResult((object)suspendstate);
        }
    }
}