namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// type of transition
    /// </summary>
    public enum TransitionType {

        /// <summary>
        /// standard transition executed on node execution end
        /// </summary>
        Standard,

        /// <summary>
        /// transition executed on node execution fail
        /// </summary>
        Error,

        /// <summary>
        /// loop transition
        /// </summary>
        Loop
    }
}