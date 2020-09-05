using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Parser;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to assign result of another node to a state variable
    /// </summary>
    public class AssignStateNode : IInstanceNode {
        readonly IInstanceNode node;

        /// <summary>
        /// creates a new <see cref="AssignStateNode"/>
        /// </summary>
        /// <param name="node">node of which to assign result</param>
        /// <param name="variableName">name of variable to assign result to</param>
        public AssignStateNode(IInstanceNode node, string variableName) {
            this.node = node;
            VariableName = variableName;
        }

        /// <summary>
        /// name of variable to assign result to
        /// </summary>
        public string VariableName { get; set; }

        /// <inheritdoc />
        public string NodeName => node.NodeName;

        /// <inheritdoc />
        public List<InstanceTransition> Transitions => node.Transitions;

        /// <inheritdoc />
        public List<InstanceTransition> ErrorTransitions => node.ErrorTransitions;

        /// <inheritdoc />
        public List<InstanceTransition> LoopTransitions => node.LoopTransitions;

        /// <inheritdoc />
        public async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            object result = await node.Execute(logger, variables, state, token);
            state[VariableName] = result;
            return result;
        }
    }
}