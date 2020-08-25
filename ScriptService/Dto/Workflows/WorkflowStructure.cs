namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// arguments used to create a new workflow
    /// </summary>
    public class WorkflowStructure {

        /// <summary>
        /// name of workflow to create
        /// </summary>
        public string Name { get; set; }

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