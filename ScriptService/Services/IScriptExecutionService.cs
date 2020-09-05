using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;

namespace ScriptService.Services {

    /// <summary>
    /// executes scripts
    /// </summary>
    public interface IScriptExecutionService {

        /// <summary>
        /// executes a script by specifying its id
        /// </summary>
        /// <param name="id">id of script to execute</param>
        /// <param name="revision">revision of script to execute (optional)</param>
        /// <param name="variables">variables to pass to script</param>
        /// <param name="wait">time to wait for script to finish</param>
        /// <returns>script task information</returns>
        Task<WorkableTask> Execute(long id, int? revision=null, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// executes a script by specifying its name
        /// </summary>
        /// <param name="name">name of script to execute</param>
        /// <param name="revision">revision of script to execute (optional)</param>
        /// <param name="variables">variables to pass to script</param>
        /// <param name="wait">time to wait for script to finish</param>
        /// <returns>script task information</returns>
        Task<WorkableTask> Execute(string name, int? revision=null, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// executes script code
        /// </summary>
        /// <param name="code">code with a scope to execute</param>
        /// <param name="variables">variables to pass to script</param>
        /// <param name="wait">time to wait for script to finish</param>
        /// <returns>script task information</returns>
        Task<WorkableTask> Execute(NamedCode code, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// executes a script using the specified logger
        /// </summary>
        /// <param name="script">script to execute</param>
        /// <param name="scriptlogger">logger to use</param>
        /// <param name="variables">variables for script execution</param>
        /// <param name="token">token to be used to cancel script execution</param>
        /// <returns>script result</returns>
        Task<object> Execute(IScript script, WorkableLogger scriptlogger, IDictionary<string, object> variables, CancellationToken token);
    }
}