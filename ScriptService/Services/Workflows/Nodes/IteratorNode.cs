using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Errors;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows.Commands;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node iterating over a collection
    /// </summary>
    public class IteratorNode : InstanceNode {
        readonly IScriptCompiler compiler;

        /// <summary>
        /// creates a new <see cref="IteratorNode"/>
        /// </summary>
        /// <param name="nodeid">id of workflow node</param>
        /// <param name="nodeName">name of node</param>
        /// <param name="parameters">parameters to use</param>
        /// <param name="compiler">compiler used to compile expressions</param>
        public IteratorNode(Guid nodeid, string nodeName, IteratorParameters parameters, IScriptCompiler compiler) 
            : base(nodeid, nodeName) {
            this.compiler = compiler;
            Parameters = parameters;
        }

        /// <summary>
        /// parameters
        /// </summary>
        public IteratorParameters Parameters { get; set; }
        
        async Task<IEnumerator> CreateEnumerator(WorkflowInstanceState state, CancellationToken token) {
            IScript enumerationscript = await compiler.CompileCodeAsync(Parameters.Collection, state.Language ?? ScriptLanguage.NCScript);
            object collection = await enumerationscript.ExecuteAsync(state.Variables, token);
            if (!(collection is IEnumerable enumerable))
                throw new WorkflowException("Can not enumerate null");

            return enumerable.GetEnumerator();
        }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkflowInstanceState state, CancellationToken token) {
            IEnumerator current = state.GetNodeData<IEnumerator>(NodeId);
            if (current == null)
                state[NodeId] = current = await CreateEnumerator(state, token);

            if (current.MoveNext()) {
                state.Variables[Parameters.Item ?? "item"] = current.Current;
                return new LoopCommand();
            }

            state.RemoveNodeData(NodeId);
            return null;
        }
    }
}