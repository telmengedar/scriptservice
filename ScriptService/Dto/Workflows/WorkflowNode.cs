using System;
using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// node in a workflow
    /// </summary>
    public class WorkflowNode {

        /// <summary>
        /// id of node
        /// </summary>
        [PrimaryKey]
        public Guid Id { get; set; }

        /// <summary>
        /// id of workflow node is contained in
        /// </summary>
        [Index("workflow")]
        public long WorkflowId { get; set; }

        /// <summary>
        /// name of workflow node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// group node is part of
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// type of node
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// type specific parameters of node (json)
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// variable to store result in
        /// </summary>
        public string Variable { get; set; }
    }
}