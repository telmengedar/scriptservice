namespace ScriptService.Dto {

    /// <summary>
    /// status of a script instance
    /// </summary>
    public enum TaskStatus {

        /// <summary>
        /// execution of script was canceled
        /// </summary>
        Canceled=-2,

        /// <summary>
        /// script failed to execute
        /// </summary>
        Failure=-1,

        /// <summary>
        /// script was executed successfully
        /// </summary>
        Success=0,

        /// <summary>
        /// script is still running
        /// </summary>
        Running=1,

        /// <summary>
        /// workable is waiting for a continue trigger
        /// </summary>
        /// <remarks>
        /// only valid for workflows
        /// </remarks>
        Suspended=2
    }
}