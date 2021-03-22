using System;

namespace ScriptService.Dto {
    
    /// <summary>
    /// entry generated for performance profiling
    /// </summary>
    public class ProfilingEntry {
        
        /// <summary>
        /// node for which a sample was generated
        /// </summary>
        public Guid? NodeId { get; set; }

        /// <summary>
        /// node for which a sample was generated
        /// </summary>
        public string NodeName{ get; set; }

        /// <summary>
        /// action executed on node
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// time it took to execute action
        /// </summary>
        public TimeSpan? Time { get; set; }
    }
}