using System;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// details of a workflow transition
    /// </summary>
    public class TransitionData : Transition {

        /// <summary>
        /// id of node transition is leaving
        /// </summary>
        public Guid OriginId { get; set; }

        /// <summary>
        /// id of node to which transition leads
        /// </summary>
        public Guid TargetId { get; set; }
    }
}