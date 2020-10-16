using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to initialize state variables
    /// </summary>
    public class StartNode : InstanceNode {

        /// <summary>
        /// creates a new <see cref="StartNode"/>
        /// </summary>
        /// <param name="nodeName">name of node</param>
        /// <param name="parameters">parameters for workflow start</param>
        public StartNode(string nodeName, StartParameters parameters)
            : base(nodeName) {
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for start node
        /// </summary>
        public StartParameters Parameters { get; }

        /// <inheritdoc />
        public override Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            return Task.FromResult((object)null);
        }
    }
}