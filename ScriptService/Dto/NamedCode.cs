namespace ScriptService.Dto {

    /// <summary>
    /// code running in a specific scope
    /// </summary>
    public class NamedCode {

        /// <summary>
        /// code to execute
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// name of script to run
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// language used for script
        /// </summary>
        public ScriptLanguage Language { get; set; }
    }
}