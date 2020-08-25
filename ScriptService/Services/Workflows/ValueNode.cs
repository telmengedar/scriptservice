using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// node providing a value to the workflow
    /// </summary>
    public class ValueNode : CompiledExpressionNode {

        /// <summary>
        /// creates a new <see cref="ValueNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="value">value to provide to workflow</param>
        /// <param name="compiler">compiler used to compile generated code</param>
        public ValueNode(string name, object value, IScriptCompiler compiler)
        : base(name, compiler)
        {
            Value = value;
        }

        /// <summary>
        /// value to provide
        /// </summary>
        public object Value { get; }

        /// <inheritdoc />
        protected override string GenerateCode() {
            return $"{Value}";
        }
    }
}