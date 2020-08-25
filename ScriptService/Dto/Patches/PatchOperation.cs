namespace ScriptService.Dto.Patches {

    /// <summary>
    /// single operation of a patch process
    /// </summary>
    public class PatchOperation {

        /// <summary>
        /// creates a new <see cref="PatchOperation"/>
        /// </summary>
        public PatchOperation() {
        }

        /// <summary>
        /// creates a new <see cref="PatchOperation"/>
        /// </summary>
        /// <param name="op">operation to execute</param>
        /// <param name="path">path where to apply operation</param>
        public PatchOperation(string op, string path) {
            Op = op;
            Path = path;
        }

        /// <summary>
        /// creates a new <see cref="PatchOperation"/>
        /// </summary>
        /// <param name="op">operation to execute</param>
        /// <param name="path">path where to apply operation</param>
        /// <param name="value">value to use for operation</param>
        public PatchOperation(string op, string path, object value)
        : this(op, path) {
            Value = value;
        }

        /// <summary>
        /// creates a new <see cref="PatchOperation"/>
        /// </summary>
        /// <param name="op">operation to execute</param>
        /// <param name="from">source path if necessary for operation</param>
        /// <param name="path">path where to apply operation</param>
        /// <param name="value">value to use for operation</param>
        public PatchOperation(string op, string from, string path, object value)
        : this(op, path, value) {
            From = from;
        }

        /// <summary>
        /// operation to execute
        /// </summary>
        public string Op { get; set; }

        /// <summary>
        /// source path if necessary for operation
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// path where to apply operation
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// value to use for operation
        /// </summary>
        public object Value { get; set; }
    }
}