using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Tasks;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// method which executes a script by name
    /// </summary>
    public class ScriptNameMethod : IExternalMethod {
        readonly string scriptname;
        readonly IScriptExecutionService executor;

        /// <summary>
        /// creates a new <see cref="ScriptIdMethod"/>
        /// </summary>
        /// <param name="scriptname">name of script</param>
        /// <param name="executor">executor used to execute script</param>
        public ScriptNameMethod(string scriptname, IScriptExecutionService executor) {
            this.scriptname = scriptname;
            this.executor = executor;
        }

        /// <inheritdoc />
        public object Invoke(IVariableProvider variables, params object[] arguments) {
            WorkableTask task;
            if (arguments.Length == 0) {
                task=executor.Execute(scriptname).GetAwaiter().GetResult();
                task.Task.GetAwaiter().GetResult();
                return task.Result;
            }

            if (!(arguments.FirstOrDefault() is IDictionary scriptarguments))
                throw new InvalidOperationException("Parameters for a script call need to be a dictionary");

            if (!(scriptarguments is IDictionary<string, object> parameters)) {
                parameters=new Dictionary<string, object>();
                foreach (object key in scriptarguments.Keys) {
                    parameters[key.ToString() ?? string.Empty] = scriptarguments[key];
                }
            }

            task = executor.Execute(scriptname, parameters).GetAwaiter().GetResult();
            task.Task.GetAwaiter().GetResult();
            return task.Result;
        }
    }
}