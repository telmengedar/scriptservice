using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node executing a binary operation
    /// </summary>
    public class BinaryNode : CompiledExpressionNode {

        /// <summary>
        /// creates a new <see cref="BinaryNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for operation</param>
        /// <param name="compiler">access to code compiler</param>
        public BinaryNode(string name, BinaryOpParameters parameters, IScriptCompiler compiler) 
        : base(name, compiler)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for operation
        /// </summary>
        public BinaryOpParameters Parameters { get; }

        /// <inheritdoc />
        protected override string GenerateCode() {
            return $"{Parameters.Lhs}{Parameters.Operation.ToOperatorString()}{Parameters.Rhs}";
        }
    }
}