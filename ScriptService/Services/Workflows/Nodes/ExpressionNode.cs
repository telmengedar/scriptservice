using System;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node which executes a script
    /// </summary>
    public class ExpressionNode : InstanceNode {
        readonly IScript script;

        /// <summary>
        /// creates a new <see cref="ScriptNode"/>
        /// </summary>
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="name">name of node</param>
        /// <param name="script">script to execute</param>
        public ExpressionNode(Guid nodeid, string name, IScript script) 
        : base(nodeid, name)
        {
            this.script = script;
        }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            return await script.ExecuteAsync(state.Variables, token);
        }
    }
}