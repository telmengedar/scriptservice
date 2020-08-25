namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// sources where argument values can get be retrieved from
    /// </summary>
    public enum ArgumentSourceType {

        /// <summary>
        /// argument is read from a state variable
        /// </summary>
        StateVariable=0,

        /// <summary>
        /// constant value
        /// </summary>
        Constant=1
    }
}