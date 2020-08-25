using System.Collections.Generic;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// data for a workflow node
    /// </summary>
    public class NodeData {

        /// <summary>
        /// name of workflow node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// name of group node is part of
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// type of node
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// type specific parameters of node
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// variable to which to assign node result
        /// </summary>
        public string Variable { get; set; }
    }
}