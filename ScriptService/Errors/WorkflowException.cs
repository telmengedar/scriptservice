using System;

namespace ScriptService.Errors {

    /// <summary>
    /// exception thrown when executing a workflow
    /// </summary>
    public class WorkflowException : Exception {

        /// <summary>
        /// creates a new <see cref="WorkflowException"/>
        /// </summary>
        /// <param name="message">error message</param>
        public WorkflowException(string? message) : base(message) {
        }

        /// <summary>
        /// creates a new <see cref="WorkflowException"/>
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="innerException">error which triggered this error</param>
        public WorkflowException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}