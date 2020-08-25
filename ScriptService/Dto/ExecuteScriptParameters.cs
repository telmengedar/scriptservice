using System;
using System.Collections.Generic;

namespace ScriptService.Dto {

    /// <summary>
    /// parameters used when executing a script
    /// </summary>
    public class ExecuteScriptParameters {

        /// <summary>
        /// id of script to execute
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// name of script to execute
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// code to execute
        /// </summary>
        public NamedCode Code { get; set; }

        /// <summary>
        /// parameters for script execution
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// specifies time to wait for result
        /// </summary>
        /// <remarks>
        /// if script doesn't finish in the specified timespan, taskinformation of the running task is returned
        /// </remarks>
        public TimeSpan? Wait { get; set; }
    }
}