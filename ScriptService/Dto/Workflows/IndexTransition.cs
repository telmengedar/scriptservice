namespace ScriptService.Dto.Workflows {
    /// <summary>
    /// argument used to create a transition
    /// </summary>
    public class IndexTransition : Transition {

        /// <summary>
        /// index of node from which transition originates
        /// </summary>
        public int OriginIndex { get; set; }

        /// <summary>
        /// index of node to which transition leads
        /// </summary>
        public int TargetIndex { get; set; }
    }
}