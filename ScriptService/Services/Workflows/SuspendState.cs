using System.Collections.Generic;
using NightlyCode.Scripting.Parser;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// state used to continue suspended workflows
    /// </summary>
    public class SuspendState {
        /// <summary>
        /// creates a new <see cref="SuspendState"/>
        /// </summary>
        /// <param name="node">suspended node</param>
        /// <param name="variables">variables of executing workflow</param>
        /// <param name="state">state of suspended workflow</param>
        /// <param name="subflow">suspended sub workflow (optional)</param>
        public SuspendState(IInstanceNode node, IVariableProvider variables, IDictionary<string, object> state, SuspendState subflow=null) {
            Variables = variables;
            State = state;
            Node = node;
            Subflow = subflow;
        }

        /// <summary>
        /// script variables
        /// </summary>
        public IVariableProvider Variables { get; }

        /// <summary>
        /// current variable state
        /// </summary>
        public IDictionary<string, object> State { get; }

        /// <summary>
        /// node at which workflow was suspended
        /// </summary>
        public IInstanceNode Node { get; }

        /// <summary>
        /// suspended state of sub workflow
        /// </summary>
        public SuspendState Subflow { get; }

        public override string ToString() {
            if (Subflow != null)
                return $"{Node.NodeName}/{Subflow}";
            return Node.NodeName;
        }
    }
}