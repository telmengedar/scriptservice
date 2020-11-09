using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node instance in a workflow
    /// </summary>
    public class InstanceNode : IInstanceNode {
        /// <summary>
        /// creates a new <see cref="InstanceNode"/>
        /// </summary>
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="nodeName">name of node</param>
        public InstanceNode(Guid nodeid, string nodeName) {
            NodeId = nodeid;
            NodeName = nodeName;
        }

        /// <inheritdoc />
        public Guid NodeId { get; }

        /// <inheritdoc />
        public string NodeName { get; }

        /// <inheritdoc />
        public List<InstanceTransition> Transitions { get; } = new List<InstanceTransition>();

        /// <inheritdoc />
        public List<InstanceTransition> ErrorTransitions { get; } = new List<InstanceTransition>();

        /// <inheritdoc />
        public List<InstanceTransition> LoopTransitions { get; } = new List<InstanceTransition>();

        /// <inheritdoc />
        public virtual Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            return Task.FromResult((object)null);
        }
    }
}