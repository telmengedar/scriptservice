namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// detailed workflow information
    /// </summary>
    public class WorkflowDetails : Workflow {

        /// <summary>
        /// nodes contained in workflow
        /// </summary>
        public NodeDetails[] Nodes { get; set; }

        /// <summary>
        /// transitions of nodes contained in workflow
        /// </summary>
        public TransitionData[] Transitions { get; set; }
    }
}