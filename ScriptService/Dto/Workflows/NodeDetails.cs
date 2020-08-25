using System;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// details for a workflow node
    /// </summary>
    public class NodeDetails : NodeData {

        /// <summary>
        /// id of node
        /// </summary>
        public Guid Id { get; set; }
    }
}