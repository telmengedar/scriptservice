namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for log node
    /// </summary>
    public class LogParameters {

        /// <summary>
        /// type of log message
        /// </summary>
        public LogLevel Type { get; set; }

        /// <summary>
        /// text to log
        /// </summary>
        public string Text { get; set; }
    }
}