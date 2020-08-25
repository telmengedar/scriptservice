namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for a node of type <see cref="NodeType.Value"/>
    /// </summary>
    public class ValueParameters {

        /// <summary>
        /// value expression to provide to workflow
        /// </summary>
        public object Value { get; set; }
    }
}