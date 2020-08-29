using System;
using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// transition from a <see cref="WorkflowNode"/> to another
    /// </summary>
    public class WorkflowTransition : Transition {

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
    }
}