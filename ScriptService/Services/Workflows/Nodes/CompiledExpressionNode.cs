using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using ScriptService.Dto;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node which compiles a script expression and executes it
    /// </summary>
    public abstract class CompiledExpressionNode : InstanceNode {
        readonly IScriptCompiler compiler;
        IScript expression;

        /// <summary>
        /// creates a new <see cref="CompiledExpressionNode"/>
        /// </summary>
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="nodeName">name of node</param>
        /// <param name="compiler">compiler used to compile scripts</param>
        protected CompiledExpressionNode(Guid nodeid, string nodeName, IScriptCompiler compiler) 
            : base(nodeid, nodeName) {
            this.compiler = compiler;
        }

        /// <summary>
        /// generates code for node to execute
        /// </summary>
        /// <returns>script code to parse and execute</returns>
        protected abstract string GenerateCode();

        /// <inheritdoc />
        public override async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            expression ??= await compiler.CompileCodeAsync(GenerateCode(), ScriptLanguage.NCScript);
            object result = await expression.ExecuteAsync(state.Variables, token);
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