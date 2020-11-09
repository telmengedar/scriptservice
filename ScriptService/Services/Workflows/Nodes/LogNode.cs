using System;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
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
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="nodeName">name of node</param>
        /// <param name="compiler">compiler for script code</param>
        /// <param name="parameters">parameters for log action</param>
        public LogNode(Guid nodeid, string nodeName, IScriptCompiler compiler, LogParameters parameters) 
            : base(nodeid, nodeName) {
            this.compiler = compiler;
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for log node
        /// </summary>
        public LogParameters Parameters { get; set; }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            logparameter ??= await compiler.CompileCodeAsync(Parameters.Text, ScriptLanguage.NCScript);
            state.Logger.Log(Parameters.Type, await logparameter.ExecuteAsync<string>(state.Variables, token));
            return null;
        }
    }
}