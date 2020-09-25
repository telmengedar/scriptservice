using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to log text
    /// </summary>
    public class LogNode : InstanceNode {
        readonly IScriptCompiler compiler;
        IScript logparameter;

        /// <summary>
        /// creates a new <see cref="LogNode"/>
        /// </summary>
        /// <param name="nodeName">name of node</param>
        /// <param name="compiler">compiler for script code</param>
        /// <param name="parameters">parameters for log action</param>
        public LogNode(string nodeName, IScriptCompiler compiler, LogParameters parameters) : base(nodeName) {
            this.compiler = compiler;
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for log node
        /// </summary>
        public LogParameters Parameters { get; set; }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            logparameter ??= await compiler.CompileCodeAsync(Parameters.Text, ScriptLanguage.NCScript);
            logger.Log(Parameters.Type, await logparameter.ExecuteAsync<string>(new VariableProvider(variables, state), token));
            return null;
        }
    }
}