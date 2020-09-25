using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Errors;
using NightlyCode.Scripting.Parser;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// method used to call script
    /// </summary>
    public abstract class ScriptMethod : IExternalMethod {

        /// <summary>
        /// loads script to be executed
        /// </summary>
        /// <returns>script to be executed</returns>
        protected abstract Task<IScript> LoadScript();

        /// <inheritdoc />
        public object Invoke(IVariableProvider variables, params object[] arguments) {
            if(!(variables.GetProvider("log")?["log"] is WorkableLogger logger))
                throw new ScriptRuntimeException($"Calling a workflow as method requires an existing logger of type '{nameof(WorkableLogger)}' accessible under variable 'log'", null);

            if(!(arguments.FirstOrDefault() is IDictionary scriptarguments))
                throw new InvalidOperationException($"Parameters for a workflow call need to be a dictionary ('{arguments.FirstOrDefault()?.GetType()}')");

            if(!(scriptarguments is IDictionary<string, object> parameters)) {
                parameters = new Dictionary<string, object>();
                foreach(object key in scriptarguments.Keys) {
                    parameters[key.ToString() ?? string.Empty] = scriptarguments[key];
                }
            }

            return Task.Run(async () => {
                IScript script = await LoadScript();
                return await script.ExecuteAsync(new StateVariableProvider(parameters, new Variable("log", logger)));
            }).GetAwaiter().GetResult();

        }
    }
}