namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// arguments used to create a new workflow
    /// </summary>
    public class WorkflowStructure : WorkflowData {
        
        /// <summary>
        /// nodes to create
        /// </summary>
        public NodeData[] Nodes { get; set; }

        /// <summary>
        /// transitions to create
        /// </summary>
        public IndexTransition[] Transitions { get; set; }
    }
}