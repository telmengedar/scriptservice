namespace ScriptService.Dto.Workflows.Nodes {
    
    /// <summary>
    /// declaration for a workflow parameter
    /// </summary>
    public class ParameterDeclaration {
        
        /// <summary>
        /// name of expected parameter
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// type of expected parameter
        /// </summary>
        /// <remarks>
        /// has to be a type known by the script parser
        /// </remarks>
        public string Type { get; set; }

        /// <summary>
        /// expression to use as default if parameter is not provided
        /// </summary>
        /// <remarks>
        /// if this is null or empty, the parameter has to be provided by the caller 
        /// if there is a value it has to be a valid nc script expression
        /// </remarks>
        public string Default { get; set; }
    }
}