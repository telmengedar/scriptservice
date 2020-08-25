using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// node used to call a method on a host variable
    /// </summary>
    public class CallNode : CompiledExpressionNode {

        /// <summary>
        /// creates a new <see cref="CallNode"/>
        /// </summary>
        /// <param name="nodeName">name of node</param>
        /// <param name="parameters">parameters for method call</param>
        /// <param name="compiler">parser for script code</param>
        public CallNode(string nodeName, CallParameters parameters, IScriptCompiler compiler) : base(nodeName, compiler) {
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for node
        /// </summary>
        public CallParameters Parameters { get; }

        /// <inheritdoc />
        protected override string GenerateCode() {
            return $"${Parameters.Host}.{Parameters.Method}({string.Join(",", Parameters.Arguments)})";
        }
    }
}