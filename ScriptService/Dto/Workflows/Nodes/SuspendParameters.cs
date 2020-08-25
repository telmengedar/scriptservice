using System;

namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for suspend node
    /// </summary>
    public class SuspendParameters {

        /// <summary>
        /// timeout for suspend operation
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// variable to initialize
        /// </summary>
        public string Variable { get; set; }
    }
}