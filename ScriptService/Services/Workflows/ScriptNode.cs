using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// node which executes a script
    /// </summary>
    public class ScriptNode : InstanceNode {
        readonly IScript script;
        readonly ScriptArgument[] arguments;

        /// <summary>
        /// creates a new <see cref="ScriptNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="script">script to execute</param>
        /// <param name="parameters">parameters for script</param>
        public ScriptNode(string name, IScript script, IEnumerable<ScriptArgument> parameters) 
        : base(name)
        {
            this.script = script;
            arguments = parameters?.ToArray() ?? new ScriptArgument[0];
        }


        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            Dictionary<string, object> scriptparameters = arguments.BuildArguments(state);
            logger.Info("Executing script", string.Join("\n", scriptparameters.Select(p => $"{p.Key}: {p.Value}")));
            return await script.ExecuteAsync(new VariableProvider(variables, scriptparameters), token);
        }
    }
}