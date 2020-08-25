namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// declares a method or host to import
    /// </summary>
    public class ImportDeclaration {

        /// <summary>
        /// variable where to store import
        /// </summary>
        public string Variable { get; set; }

        /// <summary>
        /// type of import
        /// </summary>
        public ImportType Type { get; set; }

        /// <summary>
        /// name of host, script or method
        /// </summary>
        public string Name { get; set; }
    }
}