namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for nodes of type <see cref="NodeType.Iterator"/>
    /// </summary>
    public class IteratorParameters {

        /// <summary>
        /// collection to iterate
        /// </summary>
        public string Collection { get; set; }

        /// <summary>
        /// name of variable to store current item to
        /// </summary>
        public string Item { get; set; }
    }
}