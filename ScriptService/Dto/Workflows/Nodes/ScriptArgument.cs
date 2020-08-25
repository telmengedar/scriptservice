namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// argument for a script
    /// </summary>
    public readonly struct ScriptArgument {

        /// <summary>
        /// creates a new <see cref="ScriptArgument"/>
        /// </summary>
        /// <param name="name">name of argument</param>
        /// <param name="source">type how value is retrieved</param>
        /// <param name="value">argument value</param>
        public ScriptArgument(string name, ArgumentSourceType source, object value) {
            Name = name;
            Source = source;
            Value = value;
        }

        /// <summary>
        /// name of argument
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// type of argument value
        /// </summary>
        public ArgumentSourceType Source { get; }

        /// <summary>
        /// value for script
        /// </summary>
        public object Value { get; }
    }
}