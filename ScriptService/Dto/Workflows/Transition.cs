namespace ScriptService.Dto.Workflows {
    /// <summary>
    /// base data for a transition
    /// </summary>
    public class Transition {

        /// <summary>
        /// condition for transition
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// type of transition
        /// </summary>
        public TransitionType Type { get; set; }
    }
}