using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
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
        /// <param name="nodeName">name of node</param>
        /// <param name="parameters">parameters to use</param>
        /// <param name="compiler">compiler used to compile expressions</param>
        public IteratorNode(string nodeName, IteratorParameters parameters, IScriptCompiler compiler) 
            : base(nodeName) {
            this.compiler = compiler;
            Parameters = parameters;
        }

        /// <summary>
        /// parameters
        /// </summary>
        public IteratorParameters Parameters { get; set; }

        /// <summary>
        /// current item in enumeration
        /// </summary>
        public IEnumerator Current { get; private set; }

        async Task CreateEnumerator(IVariableProvider variables, CancellationToken token) {
            IScript enumerationscript = await compiler.CompileCode(Parameters.Collection);
            if (!(await enumerationscript.ExecuteAsync(variables, token) is IEnumerable enumerable))
                throw new WorkflowException("Can not enumerate null");

            Current = enumerable.GetEnumerator();
        }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            if (Current == null)
                await CreateEnumerator(new VariableProvider(variables, state), token);

            if (Current.MoveNext()) {
                state[Parameters.Item ?? "item"] = Current.Current;
                return new LoopCommand();
            }

            Current = null;
            return null;
        }
    }
}