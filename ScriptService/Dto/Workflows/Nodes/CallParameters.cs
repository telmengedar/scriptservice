namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for a method call
    /// </summary>
    public class CallParameters {

        /// <summary>
        /// host variable on which to call a method
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// method to call
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// arguments for method
        /// </summary>
        public object[] Arguments { get; set; }
    }
}