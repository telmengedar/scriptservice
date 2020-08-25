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

        /// <summary>
        /// name of node
        /// </summary>
        public string NodeName { get; }

        /// <summary>
        /// transitions processed after this node was processed
        /// </summary>
        public List<InstanceTransition> Transitions { get; } = new List<InstanceTransition>();

        /// <summary>
        /// list of transitions without condition
        /// </summary>
        public List<InstanceTransition> DefaultTransitions { get; } = new List<InstanceTransition>();

        /// <summary>
        /// transitions for error handling
        /// </summary>
        public List<InstanceTransition> ErrorTransitions { get; } = new List<InstanceTransition>();

        /// <summary>
        /// transitions for error handling without conditions
        /// </summary>
        public List<InstanceTransition> DefaultErrorTransitions { get; }= new List<InstanceTransition>();

        /// <summary>
        /// executes the node
        /// </summary>
        public virtual Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            return Task.FromResult((object)null);
        }
    }
}