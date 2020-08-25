using System;
using System.Collections.Generic;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// parameters for continuing a suspended workflow
    /// </summary>
    public class ContinueWorkflowBody {

        /// <summary>
        /// parameters to push to workflow state
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// specifies time to wait for result
        /// </summary>
        /// <remarks>
        /// if script doesn't finish in the specified timespan, taskinformation of the running task is returned
        /// </remarks>
        public TimeSpan? Wait { get; set; }
    }
}