using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node which executes a script
    /// </summary>
    public class ExpressionNode : InstanceNode {
        readonly IScript script;

        /// <summary>
        /// creates a new <see cref="ScriptNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="script">script to execute</param>
        public ExpressionNode(string name, IScript script) 
        : base(name)
        {
            this.script = script;
        }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            logger.Info("Executing expression");
            return await script.ExecuteAsync(new StateVariableProvider(variables, state), token);
        }
    }
}