namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// argument used to create a transition
    /// </summary>
    public class IndexTransition {

        /// <summary>
        /// index of node from which transition originates
        /// </summary>
        public int OriginIndex { get; set; }

        /// <summary>
        /// index of node to which transition leads
        /// </summary>
        public int TargetIndex { get; set; }

        /// <summary>
        /// condition for transition
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// determines whether this transition is used for error handling
        /// </summary>
        public bool Error { get; set; }
    }
}