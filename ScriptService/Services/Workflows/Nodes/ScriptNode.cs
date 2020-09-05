using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Scripts;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node which executes a script
    /// </summary>
    public class ScriptNode : InstanceNode {
        readonly IScriptCompiler compiler;

        /// <summary>
        /// creates a new <see cref="ScriptNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for script</param>
        /// <param name="compiler">compiler used to retrieve script instance</param>
        public ScriptNode(string name, CallWorkableParameters parameters, IScriptCompiler compiler) 
        : base(name) {
            this.compiler = compiler;
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for script execution
        /// </summary>
        public CallWorkableParameters Parameters { get; }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            CompiledScript script = await compiler.CompileScript(Parameters.Name);
            variables = Parameters.Arguments != null ? new StateVariableProvider(variables, Parameters.Arguments) : variables;
            logger.Info($"Executing script '{script.Name}'", string.Join("\n", Parameters.Arguments?.Select(p => $"{p.Key}: {p.Value}") ?? new string[0]));
            return await script.Instance.ExecuteAsync(variables, token);
        }
    }
}