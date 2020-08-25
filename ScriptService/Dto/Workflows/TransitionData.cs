using System;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// details of a workflow transition
    /// </summary>
    public class TransitionData {

        /// <summary>
        /// id of node transition is leaving
        /// </summary>
        public Guid OriginId { get; set; }

        /// <summary>
        /// id of node to which transition leads
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// condition for transition
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// determines whether this transition is used for error handling
        /// </summary>
        public bool Error { get; set; }
    }
}