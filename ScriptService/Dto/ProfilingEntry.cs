using System;
using ScriptService.Dto.Workflows;

namespace ScriptService.Dto {
    
    /// <summary>
    /// entry generated for performance profiling
    /// </summary>
    public class ProfilingEntry {

        /// <summary>
        /// workflow node is part of
        /// </summary>
        public WorkflowIdentifier Workflow { get; set; }

        /// <summary>
        /// measured node
        /// </summary>
        public NodeIdentifier Node { get; set; }

        /// <summary>
        /// time it took to execute action
        /// </summary>
        public TimeSpan Time { get; set; }
    }
}