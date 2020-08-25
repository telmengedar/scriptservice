using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <param name="variables">variables to pass to script</param>
        /// <param name="wait">time to wait for script to finish</param>
        /// <returns>script task information</returns>
        Task<WorkableTask> Execute(long id, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// executes a script by specifying its name
        /// </summary>
        /// <param name="name">name of script to execute</param>
        /// <param name="variables">variables to pass to script</param>
        /// <param name="wait">time to wait for script to finish</param>
        /// <returns>script task information</returns>
        Task<WorkableTask> Execute(string name, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// executes script code
        /// </summary>
        /// <param name="code">code with a scope to execute</param>
        /// <param name="variables">variables to pass to script</param>
        /// <param name="wait">time to wait for script to finish</param>
        /// <returns>script task information</returns>
        Task<WorkableTask> Execute(NamedCode code, IDictionary<string, object> variables = null, TimeSpan? wait = null);
    }
}