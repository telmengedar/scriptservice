namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for a node of type <see cref="NodeType.Script"/>
    /// </summary>
    public class ExecuteExpressionParameters {

        /// <summary>
        /// name of script to call
        /// </summary>
        public string Code { get; set; }
    }
}