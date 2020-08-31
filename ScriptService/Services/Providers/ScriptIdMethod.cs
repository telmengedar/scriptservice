using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Errors;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// executes a script by id
    /// </summary>
    public class ScriptIdMethod : IExternalMethod {
        readonly long scriptid;
        readonly IScriptExecutionService executor;

        /// <summary>
        /// creates a new <see cref="ScriptIdMethod"/>
        /// </summary>
        /// <param name="scriptid">id of script</param>
        /// <param name="executor">executor used to execute script</param>
        public ScriptIdMethod(long scriptid, IScriptExecutionService executor) {
            this.scriptid = scriptid;
            this.executor = executor;
        }

        /// <inheritdoc />
        public object Invoke(IVariableProvider variables, params object[] arguments) {
            WorkableTask task;
            if (arguments.Length == 0) {
                task=executor.Execute(scriptid).GetAwaiter().GetResult();
                task.Task.GetAwaiter().GetResult();
                return task.Result;
            }

            if (!(arguments.FirstOrDefault() is IDictionary scriptarguments))
                throw new InvalidOperationException($"Parameters for a script call need to be a dictionary ('{arguments.FirstOrDefault()?.GetType()}')");

            if (!(scriptarguments is IDictionary<string, object> parameters)) {
                parameters=new Dictionary<string, object>();
                foreach (object key in scriptarguments.Keys) {
                    parameters[key.ToString() ?? string.Empty] = scriptarguments[key];
                }
            }

            task = executor.Execute(scriptid, parameters).GetAwaiter().GetResult();
            task.Task.GetAwaiter().GetResult();

            if(task.Status == TaskStatus.Failure) {
                Exception error = task.Task.Exception?.InnerException ?? task.Task.Exception;
                if(error != null)
                    throw error;
                throw new WorkflowException($"Error executing script '{scriptid}'");
            }

            return task.Result;
        }
    }
}