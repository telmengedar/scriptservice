using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to assign result of another node to a state variable
    /// </summary>
    public class AssignStateNode : IInstanceNode {
        readonly IInstanceNode node;
        readonly IScript operation;
        
        /// <summary>
        /// creates a new <see cref="AssignStateNode"/>
        /// </summary>
        /// <param name="node">node of which to assign result</param>
        /// <param name="variableName">name of variable to assign result to</param>
        /// <param name="variableoperation">operation to use when assigning variable</param>
        /// <param name="compiler">compiler to use to compile variable operation</param>
        public AssignStateNode(IInstanceNode node, string variableName, VariableOperation variableoperation, IScriptCompiler compiler) {
            this.node = node;
            VariableName = variableName;
            if (variableoperation != VariableOperation.Assign)
                operation = compiler.CompileCode($"$lhs{variableoperation.ToOperatorString()}$rhs", ScriptLanguage.NCScript);
        }

        /// <summary>
        /// name of variable to assign result to
        /// </summary>
        public string VariableName { get; }

        /// <inheritdoc />
        public Guid NodeId => node.NodeId;

        /// <inheritdoc />
        public string NodeName => node.NodeName;

        /// <inheritdoc />
        public List<InstanceTransition> Transitions => node.Transitions;

        /// <inheritdoc />
        public List<InstanceTransition> ErrorTransitions => node.ErrorTransitions;

        /// <inheritdoc />
        public List<InstanceTransition> LoopTransitions => node.LoopTransitions;

        /// <inheritdoc />
        public async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            object result = await node.Execute(state, token);
            if (operation != null) {
                if (result != null) {
                    if (!state.Variables.TryGetValue(VariableName, out object lhs))
                        lhs = result is string ? "" : Activator.CreateInstance(result.GetType());
                    result = await operation.ExecuteAsync(new VariableProvider(state.Variables, new Variable("lhs", lhs), new Variable("rhs", result)), token);
                }
            }

            state.Variables[VariableName] = result;
            return result;
        }
    }
}