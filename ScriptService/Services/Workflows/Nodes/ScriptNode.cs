﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Scripts;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node which executes a script
    /// </summary>
    public class ScriptNode : InstanceNode {
        readonly IScriptCompiler compiler;

        

        /// <summary>
        /// creates a new <see cref="ScriptNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for script</param>
        /// <param name="compiler">compiler used to retrieve script instance</param>
        public ScriptNode(string name, CallWorkableParameters parameters, IScriptCompiler compiler) 
        : base(name) {
            this.compiler = compiler;
            Name = parameters.Name;
            
            if (parameters.Arguments != null) {
                foreach (KeyValuePair<string, object> kvp in parameters.Arguments)
                    Arguments[kvp.Key] = compiler.CompileCode(kvp.Value?.ToString());
            }
        }

        
        /// <summary>
        /// name of script to execute
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// arguments for script call
        /// </summary>
        IDictionary<string, IScript> Arguments { get; } = new Dictionary<string, IScript>();

        async Task<IDictionary<string, object>> GetArguments(IDictionary<string, object> state, CancellationToken token) {
            Dictionary<string, object> arguments=new Dictionary<string, object>();
            foreach (KeyValuePair<string, IScript> argument in Arguments) {
                arguments[argument.Key] = await argument.Value.ExecuteAsync(state, token);
            }

            return arguments;
        }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            CompiledScript script = await compiler.CompileScript(Name);
            IDictionary<string, object> arguments = await GetArguments(state, token);
            variables = new StateVariableProvider(variables, arguments);
            logger.Info($"Executing script '{script.Name}'", string.Join("\n", arguments.Select(p => $"{p.Key}: {p.Value}")));
            return await script.Instance.ExecuteAsync(variables, token);
        }
    }
}