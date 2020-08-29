using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Parser;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// node instance in a workflow
    /// </summary>
    public class InstanceNode : IInstanceNode {

        /// <summary>
        /// creates a new <see cref="InstanceNode"/>
        /// </summary>
        /// <param name="nodeName">name of node</param>
        public InstanceNode(string nodeName) {
            NodeName = nodeName;
        }

        /// <inheritdoc />
        public string NodeName { get; }

        /// <inheritdoc />
        public List<InstanceTransition> Transitions { get; } = new List<InstanceTransition>();

        /// <inheritdoc />
        public List<InstanceTransition> ErrorTransitions { get; } = new List<InstanceTransition>();

        /// <inheritdoc />
        public List<InstanceTransition> LoopTransitions { get; } = new List<InstanceTransition>();

        /// <inheritdoc />
        public virtual Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            return Task.FromResult((object)null);
        }
    }
}