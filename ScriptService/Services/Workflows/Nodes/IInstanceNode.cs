using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Parser;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node in an instanced workflow
    /// </summary>
    public interface IInstanceNode {

        /// <summary>
        /// name of node
        /// </summary>
        string NodeName { get; }

        /// <summary>
        /// transitions processed after this node was processed
        /// </summary>
        public List<InstanceTransition> Transitions { get; }

        /// <summary>
        /// list of transitions without condition
        /// </summary>
        public List<InstanceTransition> ErrorTransitions { get; }

        /// <summary>
        /// list of loop transitions
        /// </summary>
        public List<InstanceTransition> LoopTransitions { get; }

        /// <summary>
        /// executes the node
        /// </summary>
        public Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token);

    }
}