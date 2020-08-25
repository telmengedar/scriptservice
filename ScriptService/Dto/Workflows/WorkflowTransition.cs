using System;
using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// transition from a <see cref="WorkflowNode"/> to another
    /// </summary>
    public class WorkflowTransition {

        /// <summary>
        /// id of workflow transition is part of
        /// </summary>
        [Index("workflow")]
        public long WorkflowId { get; set; }

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