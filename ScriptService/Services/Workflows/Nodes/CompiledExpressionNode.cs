using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node which compiles a script expression and executes it
    /// </summary>
    public abstract class CompiledExpressionNode : InstanceNode {
        readonly IScriptCompiler compiler;
        IScript expression;

        protected CompiledExpressionNode(string nodeName, IScriptCompiler compiler) 
            : base(nodeName) {
            this.compiler = compiler;
        }

        /// <summary>
        /// generates code for node to execute
        /// </summary>
        /// <returns>script code to parse and execute</returns>
        protected abstract string GenerateCode();

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            expression ??= await compiler.CompileCode(GenerateCode());
            object result = await expression.ExecuteAsync(new VariableProvider(variables, state), token);
            if (result is Task task) {
                await task;
                PropertyInfo resultproperty = task.GetType().GetProperty("Result");
                if (resultproperty != null)
                    return resultproperty.GetValue(task);
                return null;
            }

            return result;
        }
    }
}